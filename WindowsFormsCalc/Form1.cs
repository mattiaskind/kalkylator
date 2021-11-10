using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


/* Jag är väldigt mycket nybörjare på C# vilket innebär att jag inte riktigt vet vad som är "best practice". Jag försöker skriva
 * koden på ett begripligt sätt med utgångspunkt i DRY-principen (don't repeat yourself) och jag är medveten
 * om att koden med största säkerhet går att refaktorera. Jag hoppas ändå att läsbarheten är tillräcklig.
 * 
 * Jag har lagt en hel del tid på att få till logiken i räknaren. Även om man kan vara säker på vilken typ av inmatning programmet
 * tar emot finns det en mängd kombinationer som behöver hanteras. Jag har försökt implementera logik som tar hand om dessa,
 * exempelvis om användaren matar in flera nollor i början, flera kommatecken eller får för sig att summera när det
 * endast finns en term. Med det sagt har jag säkert missat någon eller några varianter. Jag har testat de kombinationer jag
 * kunnat komma på och försökt att hitta lösningar till de problem som uppstått.
 *  
 * Det mesta av koden finns i den här filen men jag har även en klass som agerar struktur för varje uträkning samt
 * en klass som tar hand om räkneoperationerna. Dessa finns i separata filer. Jag har också skapat några olika metoder 
 * utöver de metoder som hanterar klick-eventen. Under tiden en uträkning genomförs sparar jag de olika delarna som 
 * egenskaper i den här klassen. Vid varje summering lagras termer, operator och summa som egenskaper i en klass varefter 
 * de läggs i en lista. På så vis kan jag spara varje uträkning och använda listan för att visa historik. 
 * Jag vet inte om det finns eklare och mer effektiva sätt att använda för att spara uträkningar än en klass men 
 * det var vad jag kom på. Jag behöver hantera olika datatyper som hör ihop på det här sättet. Jag har sneglat lite på 
 * tuples och structs men eftersom klassen löser problemet relativt smidigt har jag hållit fast vid den lösningen.
 * 
 * Jag har valt att använda ett StringBuilder-objekt när jag skapar historiken eftersom strängar inte kan muteras utan
 * måste kopieras. Det går kanske att lösa det på ett bättre sätt och jag skapar ändå en textsträng för varje iteration
 * när jag går igenom listan men det rör sig om så få element totalt så förhoppningsvis är det inte något problem.
 * 
 * En ambition som jag inte riktigt nått upp till här är väl att varje metod i huvudsak bör göra en sak. Jag skulle nog kunna
 * skapa fler mindre metoder för att dela upp logiken mer men jag tycker ändå att det fungerar som det är nu när programmet
 * är simpelt.
 * 
 * Jag har skrivit kommentarer löpande och förökt redogöra för logiken
 */

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
