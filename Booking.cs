namespace App; 

public class Booking
{
  private int _roomNumber;    
  private string _guestName;   


  public Booking(int roomNumber, string guestName)
  {
    _roomNumber = roomNumber;  
    _guestName = guestName;    
  }


  public int GetRoomNumber() { return _roomNumber; }
  public string GetGuestName() { return _guestName; }

  // byta gästens namn om receptionen stavat fel eller gästen bytt rum i samma bokning.
  // ja, det händer. det är enklare att ha en liten setter än att skapa ny booking i UI:t.
  public void SetGuestName(string newName)
  {
    _guestName = newName;
  }

  // liten hjälpmetod för utskrift/logg. 
  public string AsText()
  {
    return "Room " + _roomNumber + " -> " + _guestName;
  }
}
