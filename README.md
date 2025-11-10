# Hotell State Manager

Ett konsolprogram som används av receptionen för att se vilka rum som är lediga eller upptagna, boka gäster och markera rum för städ eller underhåll.

All information sparas lokalt i textfiler så att programmet kan fortsätta där det slutade vid nästa start.

## Funktioner (klara)
- Inloggning för personal
- Visa lediga och upptagna rum
- Boka in gäst
- Checka ut gäst
- Markera rum som tillfälligt otillgängligt

Programmet körs i terminalen med ett enkelt menysystem.

## Tekniska besult och reflektion
Jag byggde programmet för att vara stabilt men även tydligt inom ramarna för uppgiften. Den saknar komplexiteten av tidigare projekt men fokuset ligger på förståelse för grundläggande programmeringsprinciper som inkapsling, filhantering och enkel felkontroll. Koden är även gjord att vara lätt att förstå så att man kan bygga vidare på den och förklara den. 

Although...admittedly så gör alla kommentarer det lite svårare att följa med. 

## Arkitektur och struktur
Jag delade upp koden i flera klasser för att undvika röra och dubbelkod. Även i ett litet projekt så mår jag illa utan struktur.

Program.cs är bara skalet med menyloopen. I en ideal värld skulle jag lägga till en context driven meny men för en grundläggande uppgift så håller jag det, grundläggande. En bonus sak är att jag inte behöver jobba med spaghetti. 

HotelActions.cs är "hjärnan" som hanterar all logik som inloggning, bokning, utcheckning och statusändringar. Jag valde att samla allt här så programmet har ett tydligt kontrollflöde. 

Room.cs representerar ett runm och använder en enum för status, vilket minimerar stavfel och gör datan konsekvent, nice.

User.cs är enkel. e-post och lösenord räcker. e-post är mer realistiskt men så klart kan man skriva in literally vad som helst som username.

Booking.cs binder ihop gäst och rum. Jag kunde ha lagt det i Room men vill förbereda för eventuell historik eller inloggning om projektet skulle expanderas.

SaveData.cs hanterar all fillinläsning och sparning. Det är statisk för att undvika onödiga instaster och hålla ansver tydligt anvgränsat.

## Filhantering och datastruktur
Jag använda det enkla CSV-liknande textfiler för att vis agundläggande filhantering med StreamReader och StreamWriter. LINQ undveks eftersom att uppgiften sa det men även för att det är nyttigt att hantera loopar manuellt och förstå det simpla kontroll flödet.

## Felhantering och robusthet
Jag la till enkla kontroller i alla metoder, till exempel krav på inloggning vid bokning. try/catch används i huvudloopen som skydd mot oväntade fel så att programmet fortsätter köra. 
Det handlar alltså inte om avancerad exception-hantering, men visaratt jag tänker på felvägar, inte bara lyckade flöden, epic.

## Designfilosofi
Jag försökte följa principen en sak per metod och använda tidiga return-statser för att hålla koden ren och läsbar.
Målet var förutsägbar, tydlig logik snarare än "smart" kod. 

## Final words

Om du har några fler frågor om något i koden kan du alltid kontakta mig.
oskar.gyllenor@gmail.com
Tack Manuell 