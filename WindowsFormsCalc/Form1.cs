using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsCalc
{
    public partial class Form1 : Form
    {
        // NumberString är den information som visas i räknarens display under tiden användaren matar in ett tal
        private string NumberString { get; set; }
        // Number1 lagrar den första termen
        private double Number1 { get; set; }
        // Number2 lagrar den andra termen
        private double Number2 { get; set; }
        // Operator lagrar den matematiska operatorn
        private string Operator { get; set; }
        // Om det finns en summering lagras resultetet i Result
        private double Result { get; set; }
        // StartOver indikerar huruvida en ny uträkning påbörjas efter en tidigare summering
        private bool StartOver { get; set; }
        // Operations är en lista med objekt där varje uträkning sparas
        private List<MathOperation> Operations { get; set; }

        public Form1()
        {
            InitializeComponent();
            Operations = new List<MathOperation>();            
            NumberString = "0";
            Number1 = 0;
            Number2 = 0;
            Operator = "";
            Result = 0;
            StartOver = false;
        }
        // Metoden nedan anropas när användaren matar in en siffra
        private void NumberClick(object sender, EventArgs e)
        {               
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                // Sparar siffran som användaren matat in
                string digitText = clickedButton.Text;
                                
                // Kontrollerna nedanför säkerställer rätt input

                /* 
                 * Utgångspunkten är att endast ett kommatecken kan anges under en pågående inmatning
                 * Efter en summering kan det dock finnas ett kommatecken i summan samtidigt som användaren matar in
                 * ett nytt kommatecken för att ange ett helt nytt tal vilket måste vara tillåtet. Ytterligare logik nedan hanterar
                 * de fall där användaren matar in ett nytt tal efter en summering                
                */
                if (NumberString.Contains(",") && digitText == "," && StartOver == false) return;

                // 16 siffror är max i displayen
                if (NumberString.Length == 16) return;

                // Inte mer än 1 nolla i början
                if (NumberString.Length == 1 && NumberString == "0" && digitText == "0") return;

                /* 
                 * Om det finns en tidigare summering har den sparats för att eventuellt användas
                 * i nästa uträkning. Om användaren matar in en ny siffra istället är den tidigare summan inte längre
                 * aktuell.
                 */
                if (StartOver)
                {
                    // Det finns en tidigare summering men användaren vill börja om med en helt ny term.
                    // Rensa displayen som visar den pågående uträkningen
                    if (string.IsNullOrEmpty(Operator)) labelCurrentCalc.Text = "";
                    // Spara den senast inmatade siffran
                    char latestDigit = digitText[digitText.Length - 1];                    
                    
                    // Om användaren börjar om genom att mata in ett kommatecken ska en nolla läggas till
                    if (latestDigit == ',')
                    {
                        NumberString = "0" + Convert.ToString(latestDigit);
                    }
                    // Annars ska den nya siffran ersätta summan från den tidigare uträkningen
                    else
                    {
                        NumberString = Convert.ToString(digitText[digitText.Length - 1]);
                    }
                    StartOver = false;
                }               
                // Första siffran skriver över värdet i NumberString så att det inte går att skriva ex. 01
                else if (NumberString.Length == 1 && NumberString == "0" && digitText != ",")
                {
                    NumberString = digitText;                    

                // Från och med andra siffran adderas värdet i stället för att skrivas över
                } else 
                {                               
                    NumberString += digitText;
                }
                // Uppdatera displayen med det nya värdet
                labelDisplay.Text = NumberString;
            }            
        }
        // När användaren klickar på CE, dvs för att rensa displayen. Parametrar som lagrats behålls
        // men sifferdisplayen rensas
        private void ButtonClearClick(object sender, EventArgs e)
        {            
            if (NumberString.Length > 0)
            {
                ClearDisplay();
            }
        }

        // När användaren matar in en operator
        private void ButtonOpClick(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;            
            if(clickedButton != null)
            {
                // Sparar den inmatade operatorn i en lokal variabel
                string op = clickedButton.Text;

                // Sparar det senast inmatade numret, dvs. det som finns i displayen men som ännu inte lagrats som ett nummer
                double number = double.Parse(NumberString);
                
                // Kontrollera om det saknas en lagrad operator
                if(string.IsNullOrEmpty(Operator))
                {
                    // Det finns ingen operator - lagra operatorn
                    Operator = op;
                    // Eftersom det inte finns någon lagrad operator kan det inte finnas två termer
                    // Lagra den första termen/motsvarande
                    Number1 = double.Parse(NumberString);
                    // Uppdatera displayen
                    UpdateCurrentCalcDisplay();

                } else
                {
                    /*  
                    Det finns en operator lagrad vilket innebär att det också finns en lagrad term.
                    Under förutsättning att ett nytt nummer matats in utförs en räkneoperation med den lagrade termen och 
                    den som senast matades in. Uträkningen lagras i variabeln Number1 eftersom användaren ännu inte valt att summera
                    */
                    if(number != 0) Number1 = Calculator.DoOperation(Number1, number, Operator);
                                        
                    // Lagra operatorn
                    Operator = op;

                    // Uppdatera displayen som visar den pågående uträkningen
                    UpdateCurrentCalcDisplay();
                }

                // Rensa displayen som visar varje term
                ClearDisplay();

                // Felsökning
                //Console.WriteLine("EJ SUMMERINGSLÄGE");
                //Console.WriteLine("Number1: " + Number1);
                //Console.WriteLine("Number2: " + Number2);
                //Console.WriteLine("Operator: " + Operator);
                //Console.WriteLine("Result: " + Result);
            }                       
        }
        // Metoden nedan anropas när användaren klickar för att summera
        private void ButtonEqualClick(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                // Konvertera den lagrade siffersträngen till ett nummer och lagra lokalt
                double number = double.Parse(NumberString);

                // Om det inte finns någon andra term eller om det saknas en operator sker ingen uträkning...
                if (number == 0 || Operator == "") return;

                // ...annars ska en uträkning genomföras. Lagra siffersträngen som den andra termen
                Number2 = number;

                // Genomför en matematisk operation med de termer och den operator som matats in
                Result = Calculator.DoOperation(Number1, Number2, Operator);                
                
                // Eftersom en summering utförs ska displayen visa hela uträkningen, skicka true till metoden
                UpdateCurrentCalcDisplay(true);

                // Uppdatera displayen med resultatet av uträkningen
                labelDisplay.Text = Convert.ToString(Result);

                // Spara uträkningen i objektet för uträkningar
                Operations.Add(new MathOperation() { Number1 = Number1, Number2 = Number2, MathOperator = Operator, Result = Result });

                // Anropa metoden som skriver ut historiken
                UpdateHistoryDisplay();

                // Rensa aktuell uträkning
                Number1 = 0;
                Number2 = 0;
                Operator = "";
                NumberString = Convert.ToString(Result);
                Result = 0;
                StartOver = true;
            }
        }

        // Metoden rensar displayen och den lagrade siffersträngen
        private void ClearDisplay()
        {
            labelDisplay.Text = "0";            
            NumberString = "0";
        }
        // Metoden visar historiken, det vill säga tidigre uträkningar
        private void UpdateHistoryDisplay()
        {
            if (Operations.Count == 0) return;            
            StringBuilder history = new StringBuilder();
            // Gå igenom listan med de matematiska operationerna
            foreach(MathOperation operation in Operations)
            {
                // Vid varje iteration skapas en textsträng som representerar uträkningen
                string current = $"{operation.Number1}{operation.MathOperator}{operation.Number2}={operation.Result} | ";
                
                /* 
                46 tecken som längst i historiken.
                Om den aktuella uträkningen tillsammans med tidigare överskrider maximala teckenlängden
                nollställs objektet och den senast uträkningen läggs till.
                */
                if ((current.Length + history.Length) > 46)
                {
                    history.Clear();                   
                } 
                history.Append(current);
            }
            // Uppdatera historiken med den senaste informationen
            labelHistory.Text = history.ToString();
        }

        /* 
        Metoden används för att uppdatera visningen av en pågående uträkning
        Vid fullView = true ska en hel uträkning visas, vid false pågår en uträkning och användaren har ännu 
        inte valt att sumera varför endast en term och operator ska visas 
        */
        private void UpdateCurrentCalcDisplay(bool fullView = false)
        {            
            if(fullView)
            {
                // Skriver ut hela uträkningen
                labelCurrentCalc.Text = 
                    $"{Convert.ToString(Number1)} " +
                    $"{Operator} " +
                    $"{Convert.ToString(Number2)}" +
                    $" =";
            } else
            {
                // Annars pågår en uträkning och användaren har ännu inte summerat. Endast det första numret och operatorn skrivs ut
                labelCurrentCalc.Text = $"{Convert.ToString(Number1)} {Operator}";
            }
        }
        // Metoden anropas när användaren klickar på C och vill radera den pågående uträkningen.
        private void ButtonClearAllClick(object sender, EventArgs e)
        {
            Number1 = 0;
            Number2 = 0;
            Operator = "";
            Result = 0;
            NumberString = "0";
            labelCurrentCalc.Text = "";
            ClearDisplay();
        }

    }
}
