# Simulacija košarkaškog turnira na Olimpijskim igrama
## Dino Brdar za CodeBehind

Zadatak je rađen u .NET 8.0 okruženju.

Za simuliranje rezultata je korišćena random selekcija iz normalne raspodele, koja je dodeljena svakom timu u zavisnosti od vrednosti izračunatog ELO rejtinga (a koji je aproksimiran na osnovu FIBA rankinga i konstantno se ažurira nakon svakog odigranog meča).

U debug konzoli unutar VS-a se može pratiti proces rangiranja timova u grupama (ovo sam bio prinuđen napraviti s obzirom da mi je specijalni slučaj kada se u grupi napravi krug od 3 tima oduzeo pola dana). Pisani su komentari i dokumentacija za glavne metode.
Proces radi koristeći [oficijelni pravilnik sa FIBA sajta](https://www.fiba.basketball/documents/official-basketball-rules/current.pdf), odnosno:
Apendiks D.1. za prvu fazu, u redosledu:
1. Bodovi
2. Međusobni duel
3. Koš razlika iz međusobnog duela
4. Broj postignutih poena iz međusobnih duela
5. Ukupna koš razlika
6. Ukupan broj poena
7. Pozicija na FIBA rang listi

Apendiks D.5. za rangiranje od 1. do 9. mesta, u redosledu:
1. Bodovi
2. Koš razlika
3. Broj postignutih poena
4. Pozicija na FIBA rang listi

Program poseduje jedan dodatni parametar `-v` sa kojim se mogu disable-ovati vizuelni ukrasi na vrhu i dnu prozora, budući da neke konzole koriste ANSI umesto UTF-8 enkodovanja, što čini da određeni simboli budu prikazani kao upitnici (ostatak zadatka radi najnormalnije).


Napomena: Nisam bio siguran da li nam je dozvoljeno da menjamo priložene json fajlove, ali na liniji 113 u `exibitions.json` se za protivnika Brazila naveo `"POR"`: Pretpostavio sam da se mislilo na Portoriko pa sam ispravio taj unos na `"PRI"`, s obzirom da je za računanje ELO rejtinga neophodan i ELO rejting protivnika, te kako Portugal ne učestvuje na OI, a u našem zadatku nemamo FIBA ranking svih zemalja sveta, nadao sam se da nećete zameriti.
