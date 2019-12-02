using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eatm
{
    enum BenutzerAktionAuswahl
    {
        ZeigeKonto,
        ZeigeKontostand,
        GeldAbheben,
        AenderePin,
        Abmelden
    }
    enum AdminAktionAuswahl
    {
        ZeigeAlleKonten,
        LoescheKonto,
        GeldEinzahlen,
        Abmelden
    }
    class Eatm
    {
        private string _kartenEingabe;
        private string _kartenEingabeFehler;
        private string _anmeldungEingabe;
        private string _anmeldungEingabeFehler;
        private string _pinEingabe;
        private string _pinEingabeFehler;
        private string _karteNichtGefunden;
        private string _pinUngueltig;
        private string _benutzerMenu;
        private string _menuAuswahl;
        private string _menuAuswahlFehler;
        private string _betragAuszahlung;
        private string _betragAuszahlungFehler;
        private string _adminMenu;
        private string _kontoLoeschenAuswahl;
        private string _betragEinzahlung;
        private string _betragEinzahlungFehler;
        private string _kontoAuswahlBetragEinzahlung;
        private int _maxAuszahlungBetrag;

        private List<Konto> _kontoliste;
        private Dictionary<int, int> _anzahlTransaktionen;

        public void Init()
        {
            _anmeldungEingabe = @"Login: 
0 => Admin
1 => Normaler Benutzer

Ihre Auswahl: ";
            _anmeldungEingabeFehler = "Auswahl ungueltig! Bitte versuchen sie es erneut";
            _kartenEingabe = "Ihre Konotnummer:";
            _kartenEingabeFehler = "Kontonummer ungueltig! Bitte versuchen Sie es erneut";
            _pinEingabe = "Bitte geben sie ihren Pin Code ein";
            _pinEingabeFehler = "Falscher Pin Code! Versuchen sie es erneut";
            _karteNichtGefunden = "Kontonummer nicht gefunden. Bitte versuchen sie es erneut";
            _pinUngueltig = "Pin Code stimmt nicht überein";
            _menuAuswahl = "Ihre Auswahl:";
            _menuAuswahlFehler = "Falsche Auswahl! Bitte versuchen sie es erneut";
            _betragAuszahlung = "Auszsahlungsbetrag eingeben:";
            _betragAuszahlungFehler = "Ungültiger Betrag! Bitte versuchen sie es erneut";
            _kontoLoeschenAuswahl = "Welches Konto soll gelöscht werden?";
            _betragEinzahlung = "Einzahlungsbetrag eingeben bitte:";
            _betragEinzahlungFehler = "Ungültiger Betrag! Bitte versuchen sie es erneut.";
            _kontoAuswahlBetragEinzahlung = "Auf welches Konto soll die Einzahlung erfolgen?";
            _benutzerMenu = @"Operation: 
0 => Konto ansehen
1 => Kontostand anzeigen
2 => Auszahlung
3 => Pin ändern
4 => Abmelden";
            _adminMenu = @"Operation: 
0 => Alle Konten zeigen
1 => Konto löschen
2 => Betrag einzahlen
3 => Abmelden
";
            _maxAuszahlungBetrag = 1000;
            _kontoliste = new List<Konto>
            {
                new Konto() { Name = "Uschi Glas", KontoNr = 123, PinCode = 1111, Kontostand = 20000 },
                new Konto() { Name = "Tom Cruise", KontoNr = 456, PinCode = 2222, Kontostand = 15000 },
                new Konto() { Name = "Shafikur Rahman", KontoNr = 789, PinCode = 3333, Kontostand = 29000 }
            };
            _anzahlTransaktionen = new Dictionary<int, int>();
            foreach (var konto in _kontoliste)
            {
                _anzahlTransaktionen.Add(konto.KontoNr, 0);
            }
        }

        public void Start()
        {
            var auswahlEingabe = BenutzerEingabeLesen(_anmeldungEingabe, _anmeldungEingabeFehler);
            if (auswahlEingabe == 0) Admin();
            else if (auswahlEingabe == 1) Benutzer();
            else
            {
                Console.WriteLine(_anmeldungEingabeFehler);
                Start();
            }
        }

        private void Benutzer()
        {
            Konto konto = GetBenutzer();
            if (konto == null)
            {
                Console.WriteLine(_karteNichtGefunden);
                Benutzer();
            }
            bool istGueltig = PinCheck(konto);
            if (istGueltig) BenutzerAktion(konto);
            else
            {
                Console.WriteLine(_pinUngueltig);
                Benutzer();
            }
        }

        private void BenutzerAktion(Konto konto)
        {
            Console.WriteLine(_benutzerMenu);
            var auswahl = BenutzerEingabeLesen(_menuAuswahl, _menuAuswahlFehler);
            switch (auswahl)
            {
                case (int)BenutzerAktionAuswahl.ZeigeKonto:
                   ZeigeKontoDetails(konto);
                   break;
                case (int)BenutzerAktionAuswahl.ZeigeKontostand:
                    ZeigeKontostand(konto);
                    break;
                case (int)BenutzerAktionAuswahl.GeldAbheben:
                    GeldAbheben(konto);
                    break;
                case (int)BenutzerAktionAuswahl.AenderePin:
                    AenderePin(konto);
                    break;
                case (int)BenutzerAktionAuswahl.Abmelden:
                    Abmelden();
                    break;
                default:
                    Console.WriteLine(_menuAuswahlFehler);
                    BenutzerAktion(konto);
                    break;
            }
        }

        private void Abmelden()
        {
            Console.WriteLine("Abmeldung erfolgreich");
            Console.WriteLine("---------------------");
            Start();
        }

        private void AenderePin(Konto konto)
        {
            Console.WriteLine("-----Pin code Änderung-----");
            var neuerPinCode = BenutzerEingabeLesen(_pinEingabe, _pinEingabeFehler);
            konto.PinCode = neuerPinCode;
            Console.WriteLine("Pin Code Änderung erfolgreich");
            Console.WriteLine("-----------------------");
            BenutzerAktion(konto);
        }

        private void GeldAbheben(Konto konto)
        {
            bool transactionStatus = CheckTransactionEligibility(konto);
            if (!transactionStatus) BenutzerAktion(konto);

            var betrag = BenutzerEingabeLesen(_betragAuszahlung, _betragAuszahlungFehler);
            int betragStatus = CheckbetragEligibility(konto, betrag);
            if(betragStatus == 0) BenutzerAktion(konto);

            _anzahlTransaktionen[konto.KontoNr] += 1;
            konto.Kontostand -= betrag;
            Console.WriteLine("You have successfully withdrawn {0}. Your new konto balance is {1}", betrag, konto.Kontostand);
            BenutzerAktion(konto);
        }

        private int CheckbetragEligibility(Konto konto, int betrag)
        {
            if (betrag > konto.Kontostand)
            {
                Console.WriteLine("You dont have enough balance to make that transaction");
                return 0;
            }
            if (betrag > _maxAuszahlungBetrag)
            {
                Console.WriteLine("You can't withdraw more than 1000");
                return 0;
            }
            return betrag;
        }

        private bool CheckTransactionEligibility(Konto konto)
        {
            if (_anzahlTransaktionen[konto.KontoNr] >= 3)
            {
                Console.WriteLine("-----------------------");
                Console.WriteLine("heutiges Limit erreicht");
                return false;
            }
            return true;
        }

        private void ZeigeKontostand(Konto konto)
        {
            Console.WriteLine("----Kontostand----");
            Console.WriteLine("Aktuell: "+konto.Kontostand);
            Console.WriteLine("----------------------");
            BenutzerAktion(konto);
        }

        private void ZeigeKontoDetails(Konto konto)
        {
            Console.WriteLine("----konto Details-----");
            Console.WriteLine("Name: " +konto.Name);
            Console.WriteLine("Kontonummer: " + konto.KontoNr);
            Console.WriteLine("Pin Code: " + konto.PinCode);
            Console.WriteLine("Kontostand: " + konto.Kontostand);
            Console.WriteLine("----------------------");
            BenutzerAktion(konto);
        }

        private bool PinCheck(Konto konto)
        {
            var pinCode = BenutzerEingabeLesen(_pinEingabe, _pinEingabeFehler);
            if (pinCode == konto.PinCode) return true;
            return false;
        }

        private Konto GetBenutzer()
        {
            var cardNumber = BenutzerEingabeLesen(_kartenEingabe, _kartenEingabeFehler);
            foreach (Konto normalUser in _kontoliste)
                if (cardNumber == normalUser.KontoNr) return normalUser;
            return null;
        }

        private void Admin()
        {
            Console.WriteLine(_adminMenu);
            var choice = BenutzerEingabeLesen(_menuAuswahl, _menuAuswahlFehler);
            switch (choice)
            {
                case (int)AdminAktionAuswahl.ZeigeAlleKonten:
                    ZeigeAlleKontenDetails();
                    break;
                case (int)AdminAktionAuswahl.LoescheKonto:
                    if (_kontoliste.Count > 0) LoescheKonto();
                    Console.WriteLine("No konto have to delete");
                    Admin();
                    break;
                case (int)AdminAktionAuswahl.GeldEinzahlen:
                    Depositbetrag();
                    break;
                case (int)AdminAktionAuswahl.Abmelden:
                    Abmelden();
                    break;
                default:
                    Console.WriteLine(_menuAuswahlFehler);
                    Admin();
                    break;
            }
        }

        private void Depositbetrag()
        {
            int i = 1;
            foreach (var konto in _kontoliste)
            {
                Console.WriteLine(i++ + ". Name: {0} Kontonummer: {1} Pin Code: {2} Kontostand: {3}", konto.Name, konto.KontoNr, konto.PinCode, konto.Kontostand);
            }
            var serial = BenutzerEingabeLesen(_kontoAuswahlBetragEinzahlung, _menuAuswahlFehler) - 1;
            if (serial < 0) Depositbetrag();
            if (_kontoliste.Count > serial)
            {
                var betrag = BenutzerEingabeLesen(_betragEinzahlung, _betragEinzahlungFehler);
                _kontoliste[serial].Kontostand = _kontoliste[serial].Kontostand + betrag;
                Console.WriteLine("Einzahlung von {0} erfolgreich. Neuer Kontostand beträgt {1}", betrag, _kontoliste[serial].Kontostand);
                Admin();
            }
            else
            {
                Console.WriteLine(_menuAuswahlFehler);
                Admin();
            }
            
        }

        private void LoescheKonto()
        {
            int i = 1;
            foreach (var konto in _kontoliste)
            {
                Console.WriteLine(i++ +". Full Nmae: {0} Card Number: {1} Pin Code: {2} Balance: {3}", konto.Name, konto.KontoNr, konto.PinCode, konto.Kontostand);
            }
            var serial = BenutzerEingabeLesen(_kontoLoeschenAuswahl, _menuAuswahlFehler) - 1;
            if(serial < 0) LoescheKonto();
            if (_kontoliste.Count > serial)
            {
                _kontoliste.RemoveAt(serial);
                Console.WriteLine("konto deleted successfully");
                Admin();
            }
            else
            {
                Console.WriteLine(_menuAuswahlFehler);
                Admin();
            }
        }

        private void ZeigeAlleKontenDetails()
        {
            Console.WriteLine("----------------------Alle Konten-----------------------");
            foreach (var konto in _kontoliste)
            {
                Console.WriteLine("Name: {0} Kontonummer: {1} Pin Code: {2} Kontostand: {3}", konto.Name, konto.KontoNr, konto.PinCode, konto.Kontostand);    
            }
            Console.WriteLine("------------------------------------------------------------------\n");
            Admin();
        }

        private int BenutzerEingabeLesen(string prompt, string error)
        {
            Console.WriteLine(prompt);
            var input = Console.ReadLine();
            try
            {
                return Convert.ToInt32(input);
            }
            catch (Exception)
            {
                Console.WriteLine(error);
                return BenutzerEingabeLesen(prompt, error);
            }
        }
    }
}
