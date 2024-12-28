using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MergeSort
{
    public partial class MainWindow : Window
    {
        private List<int> szamok = new List<int>(); // A generált számok tárolására
        private int sebessegMilliszekundum = 1000; // Animáció sebessége

        public MainWindow()
        {
            InitializeComponent(); // Ablak inicializálása
        }

        private void GombGeneralas_Click(object sender, RoutedEventArgs e) // Generálás gomb eseménykezelője
        {
            szamok = GeneralVeletlenSzamok(15, 1, 300); // Véletlen számok generálása
            OszlopokKirajzolasa(szamok, 0, szamok.Count - 1, Brushes.Blue); // Oszlopok kirajzolása alapállapotban
        }

        private async void GombInditas_Click(object sender, RoutedEventArgs e) // Indítás gomb eseménykezelője
        {
            await MergeSortVizualis(szamok, 0, szamok.Count - 1); // Aszinkron Merge Sort vizualizáció
        }

        private List<int> GeneralVeletlenSzamok(int darab, int min, int max) // Véletlen számok generálása
        {
            Random veletlen = new Random(); // Random objektum létrehozása
            return Enumerable.Range(min, max - min + 1) // Tartomány generálása
                             .OrderBy(x => veletlen.Next()) // Véletlenszerű sorrend kialakítása
                             .Take(darab) // Csak a kívánt mennyiség kivétele
                             .ToList(); // Lista visszaadása
        }

        private void OszlopokKirajzolasa(List<int> lista, int bal, int jobb, Brush szin) // Oszlopok kirajzolása
        {
            OszlopCanvas.Children.Clear(); // Korábbi elemek törlése a Canvas-ról
            double teljesSzelesseg = OszlopCanvas.ActualWidth; // Canvas teljes szélessége
            double oszlopSzelesseg = teljesSzelesseg / lista.Count; // Oszlop szélessége

            for (int i = bal; i <= jobb; i++) // Tartomány elemeinek kirajzolása
            {
                Rectangle oszlop = new Rectangle
                {
                    Width = oszlopSzelesseg - 2, // Oszlop szélessége
                    Height = lista[i], // Oszlop magassága
                    Fill = szin, // Oszlop színe
                    Stroke = Brushes.Black, // Oszlop keretszíne
                    StrokeThickness = 1 // Keret vastagsága
                };

                Canvas.SetLeft(oszlop, i * oszlopSzelesseg); // Oszlop helyzete vízszintesen
                Canvas.SetBottom(oszlop, 0); // Oszlop helyzete függőlegesen

                TextBlock oszlopCimke = new TextBlock
                {
                    Text = lista[i].ToString(), // Az oszlop értéke
                    Foreground = Brushes.Black, // Szöveg színe
                    FontSize = 10, // Szöveg mérete
                    TextAlignment = TextAlignment.Center // Szöveg középre igazítása
                };

                Canvas.SetLeft(oszlopCimke, i * oszlopSzelesseg + oszlopSzelesseg / 4); // Szöveg vízszintes pozíciója
                Canvas.SetTop(oszlopCimke, OszlopCanvas.ActualHeight - lista[i] - 20); // Szöveg függőleges pozíciója

                OszlopCanvas.Children.Add(oszlop); // Oszlop hozzáadása a Canvas-hoz
                OszlopCanvas.Children.Add(oszlopCimke); // Szöveg hozzáadása a Canvas-hoz
            }
        }

        private async Task MergeSortVizualis(List<int> lista, int bal, int jobb) // Merge Sort vizualizáció
        {
            if (bal < jobb) // Ellenőrizzük, hogy van-e még felosztható tartomány
            {
                int kozep = (bal + jobb) / 2; // Középső index kiszámítása

                // Felosztás vizualizálása
                OszlopokKirajzolasa(lista, bal, kozep, Brushes.Orange); // Bal rész kiemelése narancssárgával
                OszlopokKirajzolasa(lista, kozep + 1, jobb, Brushes.LightBlue); // Jobb rész kiemelése világoskékkel
                await Task.Delay(sebessegMilliszekundum); // Késleltetés az animációhoz

                // Bal oldal rendezése
                await MergeSortVizualis(lista, bal, kozep);

                // Jobb oldal rendezése
                await MergeSortVizualis(lista, kozep + 1, jobb);

                // Két rész összeillesztése
                await Merge(lista, bal, kozep, jobb);
            }
        }

        private async Task Merge(List<int> lista, int bal, int kozep, int jobb) // Két rendezett rész összeillesztése
        {
            int balMeret = kozep - bal + 1; // Bal oldali rész mérete
            int jobbMeret = jobb - kozep; // Jobb oldali rész mérete

            List<int> balResz = new List<int>(lista.GetRange(bal, balMeret)); // Bal rész másolása
            List<int> jobbResz = new List<int>(lista.GetRange(kozep + 1, jobbMeret)); // Jobb rész másolása

            int i = 0, j = 0, k = bal; // Indexek inicializálása

            while (i < balMeret && j < jobbMeret) // Amíg mindkét részben van elem
            {
                // Összehasonlítás kiemelése
                OszlopokKirajzolasa(lista, bal, jobb, Brushes.Yellow); // Összehasonlítás kiemelése
                await Task.Delay(sebessegMilliszekundum / 2); // Késleltetés az animációhoz

                if (balResz[i] <= jobbResz[j]) // Bal rész eleme kisebb vagy egyenlő
                {
                    lista[k] = balResz[i]; // Bal rész elemének átvétele
                    i++; // Bal index növelése
                }
                else // Jobb rész eleme kisebb
                {
                    lista[k] = jobbResz[j]; // Jobb rész elemének átvétele
                    j++; // Jobb index növelése
                }
                k++; // Következő pozíció

                OszlopokKirajzolasa(lista, bal, jobb, Brushes.Purple); // Folyamatban lévő összeillesztés
                await Task.Delay(sebessegMilliszekundum / 2); // Késleltetés az animációhoz
            }

            while (i < balMeret) // Ha maradt elem a bal részben
            {
                lista[k] = balResz[i]; // Bal rész elemének átvétele
                i++;
                k++;

                OszlopokKirajzolasa(lista, bal, jobb, Brushes.Green); // Kiemelés zöld színnel
                await Task.Delay(sebessegMilliszekundum / 2); // Késleltetés az animációhoz
            }

            while (j < jobbMeret) // Ha maradt elem a jobb részben
            {
                lista[k] = jobbResz[j]; // Jobb rész elemének átvétele
                j++;
                k++;

                OszlopokKirajzolasa(lista, bal, jobb, Brushes.Green); // Kiemelés zöld színnel
                await Task.Delay(sebessegMilliszekundum / 2); // Késleltetés az animációhoz
            }
        }

        private void SebessegCsuszka_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) // Sebesség csúszka értékváltozása
        {
            sebessegMilliszekundum = (int)SebessegCsuszka.Value; // Sebesség beállítása
        }
    }
}
