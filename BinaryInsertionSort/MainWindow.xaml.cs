using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BinaryInsertionSort
{
    public partial class MainWindow : Window
    {
        private List<int> szamok = new List<int>(); // A generált számok tárolására
        private int sebessegMilliszekundum = 1000; // Animáció sebessége
        private int jelenlegiIndex = 1; // Aktuális rendezendő elem indexe

        public MainWindow()
        {
            InitializeComponent(); // Ablak inicializálása
        }

        private void GombGeneralas_Click(object sender, RoutedEventArgs e) // Generálás gomb eseménykezelője
        {
            szamok = GeneralVeletlenSzamok(15, 1, 300); // Véletlen számok generálása
            jelenlegiIndex = 1; // Indítás a második elemtől
            OszlopokKirajzolasa(); // Oszlopok kirajzolása
        }

        private async void GombInditas_Click(object sender, RoutedEventArgs e) // Indítás gomb eseménykezelője
        {
            await BinaryInsertionSortVizualis(); // Aszinkron bináris beillesztéses rendezés
        }

        private void GombLepesenkent_Click(object sender, RoutedEventArgs e) // Lépésenként gomb eseménykezelője
        {
            BinaryInsertionSortLepesenkent(); // Bináris beillesztéses rendezés lépésenként
        }

        private List<int> GeneralVeletlenSzamok(int darab, int min, int max) // Véletlen számok generálása
        {
            Random veletlen = new Random(); // Random objektum létrehozása
            return Enumerable.Range(min, max - min + 1) // Tartomány generálása
                             .OrderBy(x => veletlen.Next()) // Véletlenszerű sorrend kialakítása
                             .Take(darab) // Csak a kívánt mennyiség kivétele
                             .ToList(); // Lista visszaadása
        }

        private void OszlopokKirajzolasa() // Oszlopok kirajzolása Canvas-ra
        {
            OszlopCanvas.Children.Clear(); // Korábbi elemek törlése a Canvas-ról
            double oszlopSzelesseg = OszlopCanvas.ActualWidth / szamok.Count; // Oszlop szélességének kiszámítása

            for (int i = 0; i < szamok.Count; i++) // Minden számhoz tartozó oszlop létrehozása
            {
                Rectangle oszlop = new Rectangle
                {
                    Width = oszlopSzelesseg - 2, // Oszlop szélessége
                    Height = szamok[i], // Oszlop magassága a szám értéke
                    Fill = Brushes.Blue, // Oszlop alapértelmezett színe
                    Stroke = Brushes.Black, // Oszlop keretszíne
                    StrokeThickness = 1 // Keret vastagsága
                };

                Canvas.SetLeft(oszlop, i * oszlopSzelesseg); // Oszlop helyzete vízszintesen
                Canvas.SetBottom(oszlop, 0); // Oszlop helyzete függőlegesen

                TextBlock oszlopCimke = new TextBlock
                {
                    Text = szamok[i].ToString(), // Az oszlop értéke
                    Foreground = Brushes.Black, // Szöveg színe
                    FontSize = 10, // Szöveg mérete
                    TextAlignment = TextAlignment.Center // Szöveg középre igazítása
                };

                Canvas.SetLeft(oszlopCimke, i * oszlopSzelesseg + oszlopSzelesseg / 4); // Szöveg vízszintes pozíciója
                Canvas.SetTop(oszlopCimke, OszlopCanvas.ActualHeight - szamok[i] - 20); // Szöveg függőleges pozíciója

                OszlopCanvas.Children.Add(oszlop); // Oszlop hozzáadása a Canvas-hoz
                OszlopCanvas.Children.Add(oszlopCimke); // Szöveg hozzáadása a Canvas-hoz
            }
        }

        private async Task BinaryInsertionSortVizualis() // Bináris beillesztéses rendezés vizualizációja
        {
            for (int i = 1; i < szamok.Count; i++) // Minden elemre a második index-től kezdve
            {
                int kulcs = szamok[i]; // Aktuális elem eltárolása

                // Kiemelés: az aktuális rendezendő elem zölddel
                KiemelOszlopot(i, Brushes.Green); // Kiemelés zöld színnel
                await Task.Delay(sebessegMilliszekundum / 2); // Rövid késleltetés

                int bal = 0; // Bal határ
                int jobb = i - 1; // Jobb határ

                while (bal <= jobb) // Amíg van keresési tartomány
                {
                    int kozep = (bal + jobb) / 2; // Középső elem indexe

                    // Kiemelés az összehasonlítás során
                    KiemelOszlopot(kozep, Brushes.Yellow); // Középső oszlop kiemelése sárgával
                    await Task.Delay(sebessegMilliszekundum / 2); // Rövid késleltetés

                    if (szamok[kozep] < kulcs) // Ha a középső elem kisebb a kulcsnál
                    {
                        bal = kozep + 1; // A bal határt növeljük
                    }
                    else // Egyébként
                    {
                        jobb = kozep - 1; // A jobb határt csökkentjük
                    }
                }

                szamok.RemoveAt(i); // Az aktuális elem eltávolítása az eredeti helyéről
                szamok.Insert(bal, kulcs); // Beszúrás a megfelelő helyre

                OszlopokKirajzolasa(); // Oszlopok frissítése
                await Task.Delay(sebessegMilliszekundum); // Késleltetés az animációhoz
            }
        }

        private void BinaryInsertionSortLepesenkent() // Bináris beillesztéses rendezés lépésenkénti végrehajtása
        {
            if (jelenlegiIndex < szamok.Count) // Ellenőrizzük, hogy van-e még rendezendő elem
            {
                int kulcs = szamok[jelenlegiIndex]; // Aktuális elem eltárolása

                // Kiemelés: az aktuális rendezendő elem zölddel
                KiemelOszlopot(jelenlegiIndex, Brushes.Green); // Kiemelés zöld színnel

                int bal = 0; // Bal határ
                int jobb = jelenlegiIndex - 1; // Jobb határ

                while (bal <= jobb) // Amíg van keresési tartomány
                {
                    int kozep = (bal + jobb) / 2; // Középső elem indexe

                    // Kiemelés az összehasonlítás során
                    KiemelOszlopot(kozep, Brushes.Yellow); // Középső oszlop kiemelése sárgával

                    if (szamok[kozep] < kulcs) // Ha a középső elem kisebb a kulcsnál
                    {
                        bal = kozep + 1; // A bal határt növeljük
                    }
                    else // Egyébként
                    {
                        jobb = kozep - 1; // A jobb határt csökkentjük
                    }
                }

                szamok.RemoveAt(jelenlegiIndex); // Az aktuális elem eltávolítása az eredeti helyéről
                szamok.Insert(bal, kulcs); // Beszúrás a megfelelő helyre

                OszlopokKirajzolasa(); // Oszlopok frissítése
                jelenlegiIndex++; // Következő elem indexe
            }
            else
            {
                MessageBox.Show("Rendezés kész!"); // Értesítés a rendezés befejezéséről
                jelenlegiIndex = 1; // Index visszaállítása alaphelyzetbe
            }
        }

        private void KiemelOszlopot(int index, Brush szin) // Egy adott oszlop kiemelése
        {
            Rectangle oszlop = new Rectangle
            {
                Width = OszlopCanvas.ActualWidth / szamok.Count - 2, // Oszlop szélessége
                Height = szamok[index], // Oszlop magassága
                Fill = szin, // Oszlop színe
                Stroke = Brushes.Black, // Oszlop keretszíne
                StrokeThickness = 1 // Keret vastagsága
            };

            Canvas.SetLeft(oszlop, index * (OszlopCanvas.ActualWidth / szamok.Count)); // Oszlop vízszintes helyzete
            Canvas.SetBottom(oszlop, 0); // Oszlop függőleges helyzete

            OszlopCanvas.Children.Add(oszlop); // Kiemelt oszlop hozzáadása a Canvas-hoz
        }

        private void SebessegCsuszka_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) // Sebesség csúszka értékváltozása
        {
            sebessegMilliszekundum = (int)SebessegCsuszka.Value; // Sebesség beállítása
        }
    }
}
