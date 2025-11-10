namespace App;

using System;
using System.IO;
using System.Collections.Generic;

static class SaveData
{
  // alltid läs/skriv i samma mapp som exe:et
private static readonly string BasePath  = Directory.GetCurrentDirectory();
private static readonly string UsersFile = Path.Combine(BasePath, "users.txt");
private static readonly string RoomsFile = Path.Combine(BasePath, "rooms.txt");
    // USERS 
  // skriver hela listan av användare till fil
  // varför inte append? för då skulle gamla användare ligga kvar
  // och du får dubbletter. enklare att skriva om hela filen.
  public static void SaveUsers(List<User> users)
  {
    // StreamWriter stänger sig själv i using-blocket. clean.
    using (var writer = new StreamWriter(UsersFile, append: false))
    {
      // loopar manuellt eftersom ingen LINQ
      for (int i = 0; i < users.Count; i++)
      {
        var u = users[i];
        // csv-style. inga citationstecken eller specialformat.
        writer.WriteLine(u.GetEmail() + "," + u.GetPassword());
      }
    }
  }

  // laddar alla användare från textfilen
  // varför returnera en lista och inte array? för att listor är lättare att bygga på.
  public static List<User> LoadUsers()
  {
    var list = new List<User>();

    // om filen inte finns än (första gången man kör programmet), returnera tom lista.
    // hellre tom lista än att programmet dör på en FileNotFound.
    if (!File.Exists(UsersFile)) return list;

    using (var reader = new StreamReader(UsersFile))
    {
      string? line;
      while ((line = reader.ReadLine()) != null)
      {
        // skippar tomrader för säkerhets skull.
        if (line.Trim().Length == 0) continue;

        // delar upp raden på kommatecken. vi förväntar oss "email,password"
        var parts = line.Split(',');

        // om det inte finns två delar är filen trasig. vidare.
        if (parts.Length < 2) continue;

        var email = parts[0];
        var password = parts[1];

        // skapar en user och lägger till i listan.
        list.Add(new User(email, password));
      }
    }

    // alltid returnera något – hellre tom lista än null.
    return list;
  }

  // ROOMS

  // sparar hela listan av rum till fil
  public static void SaveRooms(List<Room> rooms)
  {
    using (var writer = new StreamWriter(RoomsFile, append: false))
    {
      for (int i = 0; i < rooms.Count; i++)
      {
        var r = rooms[i];

        // om gästen är null så skriv bara tomt fält, så att load inte får "null" som text
        string guest = r.GetCurrentGuest() == null ? "" : r.GetCurrentGuest();

        // skriver ut rad: "nummer,status,gäst"
        // statusen skrivs som enum-namnet (Available/Occupied/Maintenance)
        writer.WriteLine(r.GetNumber() + "," + r.GetStatus() + "," + guest);
      }
    }
  }

  // laddar in alla rum från fil
  public static List<Room> LoadRooms()
  {
    var list = new List<Room>();

    // samma logik: finns inte filen så får man tom lista.
    if (!File.Exists(RoomsFile)) return list;

    using (var reader = new StreamReader(RoomsFile))
    {
      string? line;
      while ((line = reader.ReadLine()) != null)
      {
        // hoppa över rad om den är helt blank
        if (line.Trim().Length == 0) continue;

        // delar upp raden i sina delar
        var parts = line.Split(',');

        // förväntar minst nummer + status
        if (parts.Length < 2) continue;

        // försök tolka rumsnumret. misslyckas det så ignorera raden.
        if (!int.TryParse(parts[0], out int number)) continue;

        // tolka status (text till enum). 
        // använder try/catch eftersom Enum.Parse kastar exception på fel input.
        RoomStatus status;
        try
        {
          status = (RoomStatus)Enum.Parse(typeof(RoomStatus), parts[1]);
        }
        catch
        {
          continue; // om dåligt värde hoppa över
        }

        // skapar rum och sätter status
        var room = new Room(number);

        // sätter statusen enligt textfilen
        if (status == RoomStatus.Available)
        {
          room.SetAvailable();
        }
        else if (status == RoomStatus.Occupied)
        {
          // om det finns ett tredje fält då är det gästen
          string guest = parts.Length >= 3 ? parts[2] : "";

          // ibland kan filen ha kommatecken i slutet eller blank gäst, ersätt med “Unknown”
          if (string.IsNullOrWhiteSpace(guest)) guest = "Unknown";

          room.SetOccupied(guest);
        }
        else
        {
          // maintenance
          room.SetMaintenance();
        }

        // lägger till i listan oavsett status
        list.Add(room);
      }
    }

    return list;
  }
}
