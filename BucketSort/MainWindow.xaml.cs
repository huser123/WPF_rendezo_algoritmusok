using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BucketSort
{
    public partial class MainWindow : Window
    {
        private List<int> szamok = new List<int>(); // A generált számok tárolására
        private int sebessegMilliszekundum = 1000; // Animáció sebessége
        private int jelenlegiBucket = 0; // Aktuális vödör index
        private List<List<int>> bucketok; // Vödrök tárolása

        public MainWindow()
        {
            InitializeComponent(); // Ablak inicializálása
        }

        private void GombGeneralas_Click(object sender, RoutedEventArgs e) // Generálás gomb eseménykezelője
        {
            szamok = GeneralVeletlenSzamok(15, 1, 300); // Véletlen számok generálása
            bucketok = null; // Vödrök újrainicializálása
            jelenlegiBucket = 0; // Aktuális vödör nullázása
            OszlopokKirajzolasa(); // Oszlopok kirajzolása
        }

        private async void GombInditas_Click(object sender, RoutedEventArgs e) // Indítás gomb eseménykezelője
        {
            await BucketSortVizualis(); // Aszinkron Bucket Sort vizualizáció indítása
        }

        private void GombLepesenkent_Click(object sender, RoutedEventArgs e) // Lépésenként gomb eseménykezelője
        {
            BucketSortLepesenkent(); // Bucket Sort lépésenkénti végrehajtása
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

        private async Task BucketSortVizualis() // Bucket Sort vizualizáció megvalósítása
        {
            int maxValue = szamok.Max(); // Legnagyobb szám meghatározása
            int bucketCount = 10; // Vödrök száma

            bucketok = new List<List<int>>(new List<int>[bucketCount]); // Vödrök inicializálása
            for (int i = 0; i < bucketCount; i++) // Minden vödör inicializálása
            {
                bucketok[i] = new List<int>(); // Üres lista létrehozása
            }

            foreach (var szam in szamok) // Számok vödrökbe rendezése
            {
                int bucketIndex = (szam * bucketCount) / (maxValue + 1); // Vödör indexének meghatározása
                bucketok[bucketIndex].Add(szam); // Szám hozzáadása a megfelelő vödörhöz

                await FrissitBucketVizualizacio(bucketok, bucketCount); // Vödrök kirajzolása
                await Task.Delay(sebessegMilliszekundum); // Késleltetés az animációhoz
            }

            szamok.Clear(); // Az eredeti lista ürítése

            foreach (var bucket in bucketok) // Vödrök rendezése és összefűzése
            {
                bucket.Sort(); // Egyes vödrök rendezése
                szamok.AddRange(bucket); // Rendezett elemek hozzáadása az eredeti listához

                await FrissitBucketVizualizacio(bucketok, bucketCount); // Vödrök frissítése
                await Task.Delay(sebessegMilliszekundum); // Késleltetés az animációhoz
            }

            OszlopokKirajzolasa(); // Végleges rendezett állapot kirajzolása
        }

        private async Task FrissitBucketVizualizacio(List<List<int>> bucketok, int bucketCount) // Vödrök frissítése és kirajzolása
        {
            OszlopCanvas.Children.Clear(); // Canvas ürítése
            double bucketSzelesseg = OszlopCanvas.ActualWidth / bucketCount; // Vödör szélességének meghatározása
            double bucketMagassag = 350; // Rögzített magasság a vödrök számára

            for (int i = 0; i < bucketCount; i++) // Vödrök kirajzolása
            {
                double xPozicio = i * bucketSzelesseg; // Vödör vízszintes pozíciója
                double yPozicio = 50; // Vödör függőleges eltolása

                TextBlock vodorLabel = new TextBlock
                {
                    Text = $"Vödör {i + 1}\n({(i * (300 / bucketCount))} - {((i + 1) * (300 / bucketCount) - 1)})", // Vödör száma és tartománya
                    Foreground = Brushes.Black, // Szöveg színe
                    FontSize = 12, // Szöveg mérete
                    TextAlignment = TextAlignment.Center // Középre igazítás
                };

                Canvas.SetLeft(vodorLabel, xPozicio + bucketSzelesseg / 4); // Szöveg vízszintes pozíciója
                Canvas.SetTop(vodorLabel, 10); // Szöveg függőleges pozíciója
                OszlopCanvas.Children.Add(vodorLabel); // Szöveg hozzáadása a vászonhoz

                double elemSzelesseg = bucketSzelesseg / 3; // Elemek szélessége
                double elemMagassag = 30; // Elemmagasság rögzítve

                for (int j = 0; j < bucketok[i].Count; j++) // Számok kirajzolása a vödörben
                {
                    Rectangle elem = new Rectangle
                    {
                        Width = elemSzelesseg - 5, // Szélesség kis hézaggal
                        Height = elemMagassag, // Magasság rögzítve
                        Fill = Brushes.Blue, // Szín kék
                        Stroke = Brushes.Black, // Keret fekete
                        StrokeThickness = 1 // Keret vastagsága
                    };

                    Canvas.SetLeft(elem, xPozicio + (j % 3) * elemSzelesseg); // Elem vízszintes pozíciója
                    Canvas.SetTop(elem, yPozicio + (j / 3) * (elemMagassag + 5)); // Elem függőleges pozíciója
                    OszlopCanvas.Children.Add(elem); // Elem hozzáadása a vászonhoz

                    TextBlock elemLabel = new TextBlock
                    {
                        Text = bucketok[i][j].ToString(), // Szám értéke
                        Foreground = Brushes.White, // Szöveg színe
                        FontSize = 10, // Szöveg mérete
                        TextAlignment = TextAlignment.Center // Szöveg középre igazítása
                    };

                    Canvas.SetLeft(elemLabel, xPozicio + (j % 3) * elemSzelesseg + 5); // Szöveg vízszintes pozíciója
                    Canvas.SetTop(elemLabel, yPozicio + (j / 3) * (elemMagassag + 5) + 5); // Szöveg függőleges pozíciója
                    OszlopCanvas.Children.Add(elemLabel); // Szöveg hozzáadása a vászonhoz
                }
            }

            await Task.Delay(sebessegMilliszekundum); // Rövid késleltetés az animációhoz
        }

        private void BucketSortLepesenkent() // Bucket Sort lépésenkénti megvalósítása
        {
            // Hasonló logika az aszinkron verzióhoz, lépésenkénti frissítéssel
        }

        private void SebessegCsuszka_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) // Sebesség csúszka értékváltozása
        {
            sebessegMilliszekundum = (int)SebessegCsuszka.Value; // Sebesség beállítása
        }
    }
}
