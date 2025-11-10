namespace App;

using System;

// minimalt program: skriver meny, delegerar allt till HotelActions.
// håller Program.cs tunn så man slipper spagetti.
internal class Program
{
  static void Main()
  {
    var actions = new HotelActions(); // central logik

    bool running = true;
    while (running)
    {
      Console.WriteLine();
      Console.WriteLine("=== Hotell State Manager ===");
      Console.WriteLine("[1] Logga in");
      Console.WriteLine("[2] Visa lediga rum");
      Console.WriteLine("[3] Visa upptagna rum");
      Console.WriteLine("[4] Boka gäst");
      Console.WriteLine("[5] Checka ut");
      Console.WriteLine("[6] Markera underhåll");
      Console.WriteLine("[7] Återställ från underhåll");
      Console.WriteLine("[8] Logga ut");
      Console.WriteLine("[9] Avsluta");
      Console.Write("Val: ");

      string? input = Console.ReadLine();
      Console.WriteLine();

      try
      {
        switch (input)
        {
          case "1": actions.LogIn(); break;
          case "2": actions.ShowAvailableRooms(); break;
          case "3": actions.ShowOccupiedRooms(); break;
          case "4": actions.BookGuest(); break;
          case "5": actions.CheckOut(); break;
          case "6": actions.SetMaintenance(); break;
          case "7": actions.ClearMaintenance(); break;
          case "8": actions.LogOut(); break;
          case "9": running = false; break;
          default:
            Console.WriteLine("Ogiltigt val.");
            break;
        }
      }
      catch (InvalidOperationException)
      {
        // RequireLogin kastar här. jag visar redan meddelandet i metoden, så vi håller tyst här.
      }
      catch (Exception ex)
      {
        // safety net. i en övning: skriv bara ut felet så man ser vad som hände.
        Console.WriteLine("Fel: " + ex.Message);
      }
    }

    Console.WriteLine("Stänger programmet. Hej.");
  }
}
