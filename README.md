# Simulacija košarkaškog turnira na Olimpijskim igrama
## Dino Brdar za CodeBehind praksu

- Zadatak je rađen u .NET 8.0 okruženju - nisu instalirani nikakvi dodatni NuGet paketi.

- Za simuliranje rezultata je korišćena random selekcija iz normalne raspodele, koja je dodeljena svakom timu u zavisnosti od vrednosti izračunatog ELO rejtinga (a koji je aproksimiran na osnovu FIBA rankinga i konstantno se ažurira nakon svakog odigranog meča).

- Moguće je da meč ode u produžetke. Ako se to desi, pored rezultata će pisati `[OT]` ako je meč otišao u jedan, odnosno ` [_OT]` ako je meč otišao u više od jednog produžetka, gde `_` predstavlja broj produžetaka.

- U *Debug Output* konzoli unutar VS-a se može pratiti proces rangiranja timova u grupama, kao i ažuriranje ELO rejtinga svake ekipe nakon odigrane utakmice.

- Pisani su komentari i dokumentacija za glavne metode.

- Sortiranje grupa radi koristeći [oficijelni pravilnik sa FIBA sajta](https://www.fiba.basketball/documents/official-basketball-rules/current.pdf), odnosno:

Apendiks D.1. za prvu fazu, u redosledu:
1. Bodovi
2. Pobednik međusobnog duela
3. Poen razlika iz međusobnih duela
4. Broj postignutih poena iz međusobnih duela
5. Ukupna poen razlika
6. Ukupan broj poena
7. Pozicija na FIBA rang listi

Apendiks D.5. za rangiranje od 1. do 9. mesta, u redosledu:
1. Bodovi
2. Poen razlika
3. Broj postignutih poena
4. Pozicija na FIBA rang listi

- Program sadrži dodatne argumente koji se mogu navesti pri pokretanju programa. Oni se mogu primeniti u nastavku njegovog naziva (npr. unutar CMD-a `BasketballTournamentTask-cdbhnd.exe -v -t 1` - za PowerShell treba i `.\` ispred imena programa), ili unutar Visual Studija, desni klik na projekat *-> Properties -> Debug -> General -> Open Debug launch profiles UI*, a zatim uneti u polje *Command line arguments* (npr `-t -v`).

### Ovo su dodatni parametri koji su dostupni:
---
`-h`  - Pomoć

`-v`  - Isključiti ASCII artwork sa vrha i dna stranice (može praviti problem ako terminal koristi ANSI umesto UTF-8 enkodiranja)

`-t <i>`  - Korišćenje test rezultata za grupnu fazu (korisno za testiranje rangiranja tabele)
			
   `<i>` je opcioni parametar koji predstavlja različite test slučajeve za grupnu fazu.
   Ako se izostavi (ili se unese nepostojeći broj), podrazumevaće se da je izabran Actual test scenario (0).

Za vrednosti `<i>`:

  `-t` / `-t 0` -> Actual:
  - Rezultati koji su se zapravo dogodili na OI 2024. Zgodno za proveru da li je tabela u programu sortirana kao što je i zapravo bilo
  
  `-t 1` -> Test1:
  - Grupa A: Demonstrira formiranje kruga 3 tima gde presuđuje FIBA rang lista
  - Grupa B: Demonstrira formiranje kruga 3 tima gde presuđuje ukupan broj poena
  - Grupa C: Demonstrira formiranje kruga 3 tima gde presuđuje ukupna poen razlika
   
  `-t 2` -> Test2: 
  - Grupa A: Demonstrira formiranje kruga 3 tima gde presuđuje broj postignutih poena iz međusobnih duela
  - Grupa B: Demonstrira formiranje kruga 3 tima gde presuđuje poen razlika iz međusobnih duela
  - Grupa C: Demonstrira 2x dve ekipe sa istim brojem bodova gde presuđuje međusobni duel
   
  `-t 3` -> Test3:
  - Grupe A, B, C: demonstriraju sortiranje na osnovu broja bodova
  - 1 - Demonstrira da za prvoplasirane ekipe u narednoj rundi (plasman od 1. do 3. mesta) presuđuje FIBA ranking
  - 2 - Demonstrira da za drugoplasirane ekipe u narednoj rundi (plasman od 4. do 6. mesta) presuđuje broj postignutih poena
  - 3 - Demonstrira da za trećeplasirane ekipe u narednoj rundi (plasman od 7. do 9. mesta) presuđuje koš razlika


> [!WARNING]
> 
> - `exibitions.json`, linija 113 u originalnom fajlu: za protivnika Brazila stoji `"POR"`: Pretpostavio sam da se mislilo na Portoriko pa sam ispravio taj unos na `"PRI"`. S obzirom da je za računanje ELO rejtinga neophodan i ELO rejting protivnika, te kako Portugal (čiji je `POR` zapravo ISO kôd) ne učestvuje na OI, a u našem zadatku nemamo FIBA ranking svih zemalja sveta, traženje tima koji ne postoji bi vratilo `NullException`.
> 
> - `README.md`, linija 62: za ukrštanje parova polufinala piše da su "parovi nastali ukrštanjem šešira `D i E` ukrštaju sa parovima nastalim ukrštanjem šešira `F i G`", a u prethodnoj rečenici kaže "`D` se nasumično ukrštaju sa > timovima iz šešira `G`, a timovi iz šešira `E` sa timovima iz šešira `F`". Na osnovu primera koji se zapravo desio na OI da se zaključiti da se parovi četvrtfinala formiraju tako što se izvlače parovi iz šešira `D i G` i `E i F`, a > zatim u polufinalu pobednici `D/G` idu na pobednike `E/F`. Ovo ima smisla jer se na taj način izbegava da dva najbolje plasirana tima idu jedan na drugog već u polufinalu.
> - Napravio sam sitnu ispravku nakon roka za predaju koja ni na koji način ne utiče na funkcionalnost programa, već Vam samo omogućava fleksibilnost pri pregledanju zadatka. Do sad je zadatak mogao da se potera samo iz Visual Studija, međutim sad može i da se export-uje sadržaj `Debug` ili `Release` foldera bilo gde na disk i da se potera odatle nezavisno kroz PowerShell i CMD. Ako to iz bilo kog razloga pravi problem zbog roka za predaju, naravno povucite prvi prethodni commit [(154ceced)](https://github.com/DinoBrdar92/BasketballTournamentTask/commit/154cecedb60a6837063017a7ac34c2865643fa33).

> [!NOTE]
> Malo sam izmenio redosled reprezentacija u `groups.json` fajlu, kako bi se izgenerisao raspored utakmica po kolima identičan onom koji se desio zapravo u Parizu 2024. Sem blago izmenjenog redosleda nije dodato ništa od podataka.
