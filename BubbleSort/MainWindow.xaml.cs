using System; // Alapvető .NET funkcionalitások használata
using System.Collections.Generic; // Lista és gyűjtemény támogatás
using System.Linq; // LINQ kérések használata
using System.Threading; // A Thread osztályhoz szükséges
using System.Threading.Tasks; // Aszinkron műveletek kezelése
using System.Windows; // WPF alapvető funkcionalitások
using System.Windows.Controls; // WPF vezérlőelemek
using System.Windows.Media; // Színek és grafikák kezelése
using System.Windows.Shapes; // Alakzatok rajzolása (pl. Rectangle)

namespace Buborekrendezes // Az alkalmazás neve
{
    public partial class MainWindow : Window // Főablak osztálya, amely örökli a Window tulajdonságokat
    {
        private List<int> szamok = new List<int>(); // A generált számok tárolására szolgáló lista
        private int sebessegMilliszekundum = 1000; // Az animáció sebessége, alapértelmezésként 1 mp
        private int iLepes = 0; // Buborékrendezés külső ciklusának indexe
        private int jLepes = 0; // Buborékrendezés belső ciklusának indexe

        public MainWindow() // Konstruktor, amely inicializálja az ablakot
        {
            InitializeComponent(); // Az ablak komponenseinek inicializálása
        }

        private void GombGeneralas_Click(object sender, RoutedEventArgs e) // "Generálás" gomb kattintás kezelése
        {
            szamok = GeneralVeletlenSzamok(15, 1, 300); // 15 darab szám generálása az 1-300 tartományban
            OszlopokKirajzolasa(); // Az oszlopok kirajzolása a Canvas-ra
        }

        private async void GombInditas_Click(object sender, RoutedEventArgs e) // "Indítás" gomb kattintás kezelése
        {
            await BuborekRendezesVizualis(); // Buborékrendezés aszinkron megvalósítása
        }

        private void GombLepesenkent_Click(object sender, RoutedEventArgs e) // "Lépésenként" gomb kattintás kezelése
        {
            if (iLepes < szamok.Count - 1) // Ellenőrizzük, hogy van-e még rendezendő elem
            {
                if (jLepes < szamok.Count - iLepes - 1) // Ellenőrizzük, hogy van-e további belső ciklus
                {
                    KiemelOszlopokat(jLepes, jLepes + 1, Brushes.Green); // Zölddel jelöljük a vizsgált oszlopokat

                    if (szamok[jLepes] > szamok[jLepes + 1]) // Ha csere szükséges
                    {
                        KiemelOszlopokat(jLepes, jLepes + 1, Brushes.Red); // Pirosra váltás a cserénél
                        CsereOszlopok(jLepes, jLepes + 1); // Cseréljük az elemeket
                    }

                    KiemelOszlopokat(jLepes, jLepes + 1, Brushes.Blue); // Az oszlopok színének visszaállítása
                    jLepes++; // Lépés a belső ciklusban
                }
                else
                {
                    jLepes = 0; // Belső ciklus visszaállítása
                    iLepes++; // Külső ciklus léptetése
                }
            }
            else
            {
                MessageBox.Show("Rendezés kész!"); // Értesítés, hogy a rendezés befejeződött
                iLepes = 0; // Indexek alaphelyzetbe állítása
                jLepes = 0;
            }
        }

        private List<int> GeneralVeletlenSzamok(int darab, int min, int max) // Egyedi számok generálása adott tartományban
        {
            Random veletlen = new Random(); // Random objektum példányosítása
            return Enumerable.Range(min, max - min + 1) // Számok listájának létrehozása a tartományban
                             .OrderBy(x => veletlen.Next()) // Lista keverése random sorrendbe
                             .Take(darab) // Az első 'darab' elemet választja ki
                             .ToList(); // A lista visszaadása
        }

        private void OszlopokKirajzolasa() // Az oszlopok megrajzolása a Canvas-ra
        {
            OszlopCanvas.Children.Clear(); // Canvas törlése, hogy újrarajzolhassuk az oszlopokat

            double oszlopSzelesseg = OszlopCanvas.ActualWidth / szamok.Count; // Egy oszlop szélességének kiszámítása

            for (int i = 0; i < szamok.Count; i++) // Minden számot oszloppá alakítunk
            {
                Rectangle oszlop = new Rectangle // Oszlop (Rectangle) objektum létrehozása
                {
                    Width = oszlopSzelesseg - 2, // Oszlop szélességének beállítása
                    Height = szamok[i], // Oszlop magassága megfelel a szám értékének
                    Fill = Brushes.Blue, // Oszlop színe kék
                    Stroke = Brushes.Black, // Oszlop kerete fekete
                    StrokeThickness = 1 // Keret vastagsága 1 pixel
                };

                Canvas.SetLeft(oszlop, i * oszlopSzelesseg); // Oszlop helyzete a Canvas-on (bal oldali pozíció)
                Canvas.SetBottom(oszlop, 0); // Oszlop helyzete a Canvas-on (alsó pozíció)

                OszlopCanvas.Children.Add(oszlop); // Oszlop hozzáadása a Canvas-hoz
            }
        }

        private async Task BuborekRendezesVizualis() // Buborékrendezés vizualizációja
        {
            for (int i = 0; i < szamok.Count - 1; i++) // Külső ciklus
            {
                for (int j = 0; j < szamok.Count - i - 1; j++) // Belső ciklus
                {
                    KiemelOszlopokat(j, j + 1, Brushes.Green); // Zölddel jelöljük az összehasonlítást
                    await Task.Delay(sebessegMilliszekundum); // Késleltetés az aktuális sebesség alapján

                    if (szamok[j] > szamok[j + 1]) // Ha csere szükséges
                    {
                        KiemelOszlopokat(j, j + 1, Brushes.Red); // Piros szín jelzi a cserét
                        await Task.Delay(sebessegMilliszekundum); // Rövid késleltetés a piros szín megjelenítéséhez
                        CsereOszlopok(j, j + 1); // Cseréljük az elemeket
                        await Task.Delay(sebessegMilliszekundum); // Késleltetés a cserénél is
                    }

                    KiemelOszlopokat(j, j + 1, Brushes.Blue); // Visszaállítás alapállapotra
                }
            }
        }

        private void KiemelOszlopokat(int index1, int index2, Brush szin) // Két oszlop kiemelése adott színnel (javaslat: kódismétlés csökkentése)
        {
            (Rectangle oszlop1, Rectangle oszlop2) = GetOszlopPar(index1, index2); // Két oszlop lekérése a Canvas-ból
            if (oszlop1 != null && oszlop2 != null) // Null ellenőrzés a futásbiztonság érdekében
            {
                oszlop1.Fill = szin; // Első oszlop színe
                oszlop2.Fill = szin; // Második oszlop színe
            }
        }

        private void CsereOszlopok(int index1, int index2) // Két oszlop cseréje
        {
            int temp = szamok[index1]; // Ideiglenes változó a csere segítségére
            szamok[index1] = szamok[index2]; // Első szám helyére kerül a második
            szamok[index2] = temp; // Második szám helyére kerül az első

            OszlopokKirajzolasa(); // Az oszlopok újrarajzolása a Canvas-on
        }

        private (Rectangle, Rectangle) GetOszlopPar(int index1, int index2) // Két oszlop lekérése index alapján
        {
            Rectangle oszlop1 = OszlopCanvas.Children[index1] as Rectangle; // Első oszlop lekérése
            Rectangle oszlop2 = OszlopCanvas.Children[index2] as Rectangle; // Második oszlop lekérése

            return (oszlop1, oszlop2); // Oszlopok párosítása és visszaadása
        }

        private void SebessegCsuszka_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) // Sebesség csúszka értékváltozása
        {
            sebessegMilliszekundum = Math.Max((int)SebessegCsuszka.Value, 50); // Minimális korlát beállítása (javaslat a felhasználói hibaelkerüléshez)
        }
    }
}
