namespace App;

using System;
using System.Collections.Generic;

// den här klassen är "hjärnan" bakom menyn.
// jag samlar all state här för att slippa sprida logik i Program.cs
public class HotelActions
{
  private List<User> _users;          // personal som får logga in
  private List<Room> _rooms;          // alla rum vi hanterar
  private List<Booking> _bookings;    // speglar "vem bor var just nu" (deriveras från rum vid start)
  private User? _activeUser;          // vem är inloggad just nu

  public HotelActions()
  {
    // laddar allt från fil så vi kan fortsätta där vi slutade
    _users = SaveData.LoadUsers();
    _rooms = SaveData.LoadRooms();

    // bygger upp bookings-listan från rummen (enkelt och robust)
    _bookings = new List<Booking>();
    for (int i = 0; i < _rooms.Count; i++) // old-school loop, enklare att läsa än foreach när man ev. ska ta bort element
    {
      var r = _rooms[i];
      if (r.GetStatus() == RoomStatus.Occupied && r.GetCurrentGuest() != null) // hämtar rummet på den här positionen, enklare att läsa än att skriva _rooms[i] hela tiden
      {
        _bookings.Add(new Booking(r.GetNumber(), r.GetCurrentGuest())); // vi bryr oss bara om rum som är markerade upptagna och där en gäst faktiskt finns sparad
      }
    }

    _activeUser = null;  // ingen är inloggad vid start
  }

  // snabb väg att lägga till rum när man vill bootstrap:a data
  // behåller metoden intern här tills vi vill exponera den via meny
  private void AddRoomIfMissing(int number)
  {
    // enkel dubbelkoll
    for (int i = 0; i < _rooms.Count; i++)
    {
      if (_rooms[i].GetNumber() == number) return; // finns redan
    }
    _rooms.Add(new Room(number));
    SaveData.SaveRooms(_rooms);
  }

  //INLOGG

  public void LogIn()
  { //// om nån redan är inloggad ska vi inte släppa in en till, enkel guard
    if (_activeUser != null)
    {
      Console.WriteLine("Du är redan inloggad som " + _activeUser.GetEmail());
      return; // hoppa ut direkt
    }
    // frågar efter personalens e-post. funkar som användarnamn i systemet.
    Console.Write("Personal e-post: ");
    string? email = Console.ReadLine();
    // standardlösenord
    Console.Write("Lösenord: ");
    string? pass = Console.ReadLine();
    // basic sanity check, inga tomma strängar, annars hade nån loggat in med bara ENTER.
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pass))
    {
      Console.WriteLine("Tom inmatning. Försök igen.");
      return;
    }

    // härifrån letar vi efter användaren i listan, loopar manuellt
    for (int i = 0; i < _users.Count; i++)
    {
      var u = _users[i];
      if (u.TryLogin(email, pass))
      {
        _activeUser = u;  // logga in
        Console.WriteLine("Inloggad: " + _activeUser.GetEmail());
        return;
      }
    }

    Console.WriteLine("Fel e-post eller lösenord.");
  }

  public void LogOut()
{
  // om ingen är inloggad finns inget att logga ut
  if (_activeUser == null)
  {
    Console.WriteLine("Ingen är inloggad.");
    return;
  }

  // visar vem som loggades ut – mest för feedback till användaren
  Console.WriteLine("Utloggad: " + _activeUser.GetEmail());

  // nollställer referensen, då vet systemet att ingen session är aktiv längre
  _activeUser = null;
}

// ===================== LISTOR =====================

public void ShowAvailableRooms()
{
  RequireLogin();  // säkerhetskoll: ingen access utan inlogg
  Console.WriteLine("Lediga rum:");
  bool any = false;  // flagga för att se om vi hittade något

  // går igenom alla rum ett efter ett, inga LINQ-tricks
  for (int i = 0; i < _rooms.Count; i++)
  {
    var r = _rooms[i];  // hämtar rummet vid index i
    if (r.GetStatus() == RoomStatus.Available)  // bara lediga skrivs ut
    {
      Console.WriteLine("  " + r.GetNumber());
      any = true;
    }
  }

  // om inga rum matchade: visa en tydlig rad istället för tom lista
  if (!any) Console.WriteLine("  (inga just nu)");
}

public void ShowOccupiedRooms()
{
  RequireLogin();
  Console.WriteLine("Upptagna rum:");
  bool any = false;

  for (int i = 0; i < _rooms.Count; i++)
  {
    var r = _rooms[i];
    if (r.GetStatus() == RoomStatus.Occupied)
    {
      // null-check så vi inte skriver “null” i konsolen
      string guest = r.GetCurrentGuest() == null ? "Okänd" : r.GetCurrentGuest();
      Console.WriteLine("  " + r.GetNumber() + "  ->  " + guest);
      any = true;
    }
  }

  if (!any) Console.WriteLine("  (inga just nu)");
}

public void ShowMaintenanceRooms()
{
  RequireLogin();
  Console.WriteLine("Rum i underhåll:");
  bool any = false;

  for (int i = 0; i < _rooms.Count; i++)
  {
    var r = _rooms[i];
    if (r.GetStatus() == RoomStatus.Maintenance)
    {
      Console.WriteLine("  " + r.GetNumber());
      any = true;
    }
  }

  if (!any) Console.WriteLine("  (inga just nu)");
}

//OKNING

public void BookGuest()
{
  RequireLogin();  // ingen bokning utan inlogg

  // läser rumsnumret som text först – försöker sen tolka till int
  Console.Write("Rumsnummer att boka: ");
  string? raw = Console.ReadLine();
  int number;

  // int.TryParse returnerar false om det inte är en siffra
  if (!int.TryParse(raw, out number))
  {
    Console.WriteLine("Inte ett nummer. Avbryter.");
    return;
  }


    // hitta rummet
    Room? room = FindRoom(number);
    if (room == null)
    {
      Console.WriteLine("Rummet finns inte. Skapar och markerar som nytt ledigt rum först.");
      AddRoomIfMissing(number);
      room = FindRoom(number); // hämta igen
    }

    // bara lediga rum får bokas, annars blir livet gnälligt i receptionen
    if (room.GetStatus() != RoomStatus.Available)
    {
      Console.WriteLine("Rummet är inte ledigt just nu (" + room.GetStatus() + ").");
      return;
    }

    Console.Write("Gästens namn: ");
    string? guest = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(guest))
    {
      Console.WriteLine("Behöver ett namn. Avbryter.");
      return;
    }

    // uppdatera rummet och spara
    room.SetOccupied(guest);
    SaveData.SaveRooms(_rooms);

    // håll bookings i synk (det gör utskrifter enklare om man vill)
    _bookings.Add(new Booking(number, guest));

    Console.WriteLine("Bokat: rum " + number + " för " + guest);
  }

  public void CheckOut()
  {
    RequireLogin();

    Console.Write("Rumsnummer att checka ut: ");
    string? raw = Console.ReadLine();
    int number;
    if (!int.TryParse(raw, out number))
    {
      Console.WriteLine("Inte ett nummer. Avbryter.");
      return;
    }

    Room? room = FindRoom(number);
    if (room == null)
    {
      Console.WriteLine("Rummet finns inte.");
      return;
    }

    if (room.GetStatus() != RoomStatus.Occupied)
    {
      Console.WriteLine("Rummet är inte upptaget.");
      return;
    }

    // nollställ state
    room.SetAvailable();
    SaveData.SaveRooms(_rooms);

    // plocka bort ev. booking-koppling
    RemoveBooking(number);

    Console.WriteLine("Utcheckat: rum " + number);
  }

  //UNDERHÅLL

  public void SetMaintenance()
  {
    RequireLogin();

    Console.Write("Rumsnummer till underhåll: ");
    string? raw = Console.ReadLine();
    int number;
    if (!int.TryParse(raw, out number))
    {
      Console.WriteLine("Inte ett nummer. Avbryter.");
      return;
    }

    Room? room = FindRoom(number);
    if (room == null)
    {
      Console.WriteLine("Rummet finns inte. Skapar först, ställer i underhåll.");
      AddRoomIfMissing(number);
      room = FindRoom(number);
    }

    // om rummet är upptaget och sätts i maintenance, vi låter det passera.
    // verkligheten: man borde stoppa det. här: enkel nivå.
    room.SetMaintenance();
    SaveData.SaveRooms(_rooms);

    Console.WriteLine("Rum " + number + " markerat som underhåll.");
  }

  public void ClearMaintenance()
  {
    RequireLogin();

    Console.Write("Rumsnummer att återställa från underhåll: ");
    string? raw = Console.ReadLine();
    int number;
    if (!int.TryParse(raw, out number))
    {
      Console.WriteLine("Inte ett nummer. Avbryter.");
      return;
    }

    Room? room = FindRoom(number);
    if (room == null)
    {
      Console.WriteLine("Rummet finns inte.");
      return;
    }

    if (room.GetStatus() != RoomStatus.Maintenance)
    {
      Console.WriteLine("Rummet står inte i underhåll.");
      return;
    }

    room.SetAvailable();
    SaveData.SaveRooms(_rooms);

    Console.WriteLine("Rum " + number + " är åter ledigt.");
  }

  //HJÄLPARE

  private void RequireLogin()
  {
    if (_activeUser == null)
    {
      Console.WriteLine("Logga in först.");
      throw new InvalidOperationException("not logged in"); // enkel guard. fångas inte, men stoppar flödet
    }
  }

  private Room? FindRoom(int number)
  {
    for (int i = 0; i < _rooms.Count; i++)
    {
      if (_rooms[i].GetNumber() == number) return _rooms[i];
    }
    return null;
  }

  private void RemoveBooking(int roomNumber)
  {
    // manuell sökning och remove
    for (int i = 0; i < _bookings.Count; i++)
    {
      if (_bookings[i].GetRoomNumber() == roomNumber)
      {
        _bookings.RemoveAt(i);
        return;
      }
    }
  }
}
