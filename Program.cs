using System.Runtime.InteropServices.JavaScript;
using System.Threading.Channels;
using System.Xml;

namespace linkedList;

class Program
{
    static void Main()
    {
        
        LinkedList<Record> recordLinkedList = new();
        LinkedList<Record>.Node<Record> focusNode = null;
        
        focusNode = recordLinkedList.AddLast(new Record("First",DateOnly.FromDateTime(DateTime.Now), ["ahoj","jak se mas"]));
        recordLinkedList.AddLast(new Record("Second",DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), ["ahoj","jak se mas"]));
        recordLinkedList.AddLast(new Record("third",DateOnly.FromDateTime(DateTime.Now.AddDays(1)), ["ahoj","jak se mas"]));

        List<string> commandHistory = new();
        int commandHistoryIndex = 0;
        Dictionary<string, Command> func = new();

        func.Add("vytvorit", new Command( (args) =>
        {
            string name = "";
            do
            {
                Console.WriteLine("Zadej Nazev:");
                name = Console.ReadLine();
            } while (name == "");
            Console.WriteLine();
            Console.WriteLine("Zadej Datum:");
            var date = GetDate();
            Console.Write("\n\n");
            Console.WriteLine("Zadej Text:");
            List<string> rows = new();
            string row = Console.ReadLine();
            while(row != "uloz")
            {
                rows.Add(row);
                row = Console.ReadLine();
            }
            if (rows.Count == 0) rows.Add("");

            if (focusNode is null || focusNode.Next is null)
            {
                focusNode = recordLinkedList.AddLast(new Record(name, date, rows));
            }
            else
            {
                focusNode = recordLinkedList.AddAfter(focusNode,new Record(name, date, rows));
            }
            
            return false;
        }, "Vytvori novy zaznam"));
        func.Add("dalsi", new Command((args) =>
        {
            if (focusNode == null || focusNode.Next == null) return false;
            focusNode = focusNode.Next;
            return false;
        }, "Prejde na dalsi zaznam"));
        func.Add("predchozi", new Command((args) =>
        {
            if (focusNode == null || focusNode.Previous == null) return false;
            focusNode = focusNode.Previous;
            return false;
        }, "Prejde na predchozi zaznam"));
        func.Add("konec", new Command((args) =>
        {
            if (focusNode == null) return false;
            focusNode = recordLinkedList.Last;
            return false;
        }, "Prejde na posladni zaznam"));
        func.Add("zacatek", new Command((args) =>
        {
            if (focusNode == null) return false;
            focusNode = recordLinkedList.First;
            return false;
        }, "prejde na prvni zaznam"));
        func.Add("vypis", new Command((args) =>
        {
            if (focusNode != null)
            {
                Console.WriteLine(focusNode.Value);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Neni vytvoreny zadny zaznam");
                Console.ResetColor();
            }
            return false;
        }, "Vypise informace o zaznamu"));
        func.Add("vse", new Command((args) =>
        {
            if (focusNode == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Neni vytvoreny zadny zaznam");
                Console.ResetColor();
                return false;
            }
            var focus = recordLinkedList.First;
            while (focus != null)
            {
                if (focus == focusNode)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                Console.Write(focus.Value.Name);
                Console.ResetColor();
                focus = focus.Next;
                if (focus != null)
                {
                    Console.Write(" => ");
                }
            }
            Console.WriteLine();
            return false;
        }, "Vypise vsechny zaznam jak jdou zasebou"));
        func.Add("pocet", new Command((args) =>
        {
            Console.WriteLine("Pocet zaznamu: " + recordLinkedList.Count);
            return false;
        }, "Vypise pocet ulozenych zaznamu"));
        func.Add("smaz", new Command((args) =>
        {
            Console.Write("Opravdu chcete smazat zaznam [Y/n] ");
            var input = Console.ReadLine().ToLower();
            if (input != "y" && input != "") return false;
            
            recordLinkedList.Remove(focusNode);
            focusNode = recordLinkedList.First;
            return false;
        }, "Smaze prave vybrany zaznam"));
        func.Add("info", new Command((args) =>
        {
            Console.WriteLine("Ahoj vytej v mem programu DENIK");
            Console.WriteLine("Zde si budes moc ukladat jednotlive udalosti");
            Console.WriteLine("pokud chces zobrazit seznam prikazu napis \"help\"");
            
            return false;
        }, "Vypise info o programu"));
        func.Add("help", new Command((args) =>
        {
            Console.WriteLine("Commands:");
            foreach (var fun in func)
            {
                Console.WriteLine("  - [" + fun.Key + "] - " + fun.Value.Description);
            }
            return false;
        }, "Vypise seznam prikazu"));
        func.Add("clear", new Command((args) =>
        {
            Console.Clear();
            return false;
        }, "Vymaze celou obrazovku"));
        func.Add("skoc", new Command((args) =>
        {
            var node = recordLinkedList.First;
            bool find = false;
            while (node != null)
            {
                if (node.Value.Name == args[0])
                {
                    focusNode = node;
                    find = true;
                }
                node = node.Next;
            }

            if (!find)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Zaznam se jmenem " + args[0] + " nebyl nalezen.");
                Console.ResetColor();
            }
            return false;
        }, "Jde na konkretni zaznam", true, () =>
        {
            List<string> list = [];
            var node = recordLinkedList.First;
            while (node != null)
            {
                list.Add(node.Value.Name);
                node = node.Next;
            }
            return list.ToArray();
        }));
        func.Add("datumy", new Command((args) =>
        {
            PriorityQueue<Record, DateOnly> list = new();
            var node = recordLinkedList.First;
            while (node != null)
            {
                list.Enqueue(node.Value, node.Value.Date);
                node = node.Next;
            }

            while (list.Count > 0)
            {
                var record = list.Dequeue();
                Console.WriteLine(record.Date + " => " + record.Name );
            }
            return false;
        }, "Srovna zaznamy podle data a vypise je"));
        func.Add("exit", new Command((args) =>
        {
            return true;
        }, "Ukonci program"));



        string input = "info";
        
        while (true)
        {
            var commands = input.Split(' ');
            for (int i = 0; i < commands.Length; i++)
            {
                if (func.ContainsKey(commands[i]))
                {
                    if (func[commands[i]].HasArgs)
                    {
                        if (i + 1 < commands.Length)
                        {
                            if (func[commands[i]].Func([commands[i+1]])) return;
                        }
                        i++;
                    }
                    else
                    {
                        if (func[commands[i]].Func([])) return;
                    }
                }
                else
                {
                    if (commands[i] != "")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Prikaz " + commands[i] + " nebyl nalezen.");
                        Console.ResetColor();
                    }
                }
            }


            
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write((focusNode != null ? focusNode.Value.Name : "")  + " > ");
            Console.ResetColor();
            string line = "";
            string historyline = "";
            ConsoleKeyInfo key = Console.ReadKey(true);
            while (key.Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (line.Length > 0)
                    {
                        line = line.Substring(0, line.Length - 1);
                        Console.CursorLeft--;
                        Console.Write(" ");
                    }
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    if (commandHistoryIndex == 0)
                    {
                        historyline = line;
                    }
                    if (commandHistoryIndex < commandHistory.Count)
                    {
                        commandHistoryIndex++;
                        line = commandHistory[commandHistory.Count - commandHistoryIndex];
                        Console.CursorLeft = 0;
                        Console.Write(new string(' ', Console.WindowWidth));
                    }
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (commandHistoryIndex == 1)
                    {
                        line = historyline;
                        commandHistoryIndex--;
                        Console.CursorLeft = 0;
                        Console.Write(new string(' ', Console.WindowWidth));
                    }
                    else if (commandHistoryIndex > 0)
                    {
                        commandHistoryIndex--;
                        line = commandHistory[commandHistory.Count - commandHistoryIndex];
                        Console.CursorLeft = 0;
                        Console.Write(new string(' ', Console.WindowWidth));
                    }
                }
                else if (key.Key == ConsoleKey.Tab)
                {
                    var splith = line.Split(' ');

                    var list = func.Keys.ToArray();
                    if (splith.Length > 1 && func.Keys.Contains(splith[splith.Length - 2]) && func[splith[splith.Length - 2]].HasArgs)
                    {
                        list = func[splith[splith.Length - 2]].Args;
                    }
                    var sync = list.Where(x => x.StartsWith(splith.Last())).ToList();
                    if (sync.Count == 1)
                    {
                        splith[splith.Length - 1] = sync[0];
                        line = string.Join(' ',splith);
                    }
                    else if (sync.Count != 0)
                    {
                        Console.WriteLine();
                        for (int i = 0; i < sync.Count; i++)
                        {
                            Console.Write(sync[i] + "       ");
                        }
                        Console.WriteLine();
                    }
                }
                else
                {
                    if (!char.IsControl(key.KeyChar))
                    {
                        line += key.KeyChar;
                    }
                }
                Console.CursorLeft = 0;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write((focusNode != null ? focusNode.Value.Name : "")  + " > ");
                Console.ResetColor();
                Console.Write(line);
                key = Console.ReadKey(true);
            }

            Console.WriteLine();
            input = line.Trim();
            //input += " clear vypis" ;
            commandHistory.Add(line);
            commandHistoryIndex = 0;
        }

    }

    public static int GetKeyNumber()
    {
        int number;
        ConsoleKey input;
        do
        {
            input = Console.ReadKey(true).Key;
            if(input == ConsoleKey.Backspace) return -1;
            if(input == ConsoleKey.Enter) return 10;
        } while (!int.TryParse(input.ToString()[input.ToString().Length - 1].ToString(), out number) && number < 10);
        return number;
    }

    public static DateOnly GetDate()
    {
        var numbers = new int?[8];
        bool isOk = false;
        int index = 0;
        
        Func<int, string> format = a => numbers[a] is not null ? numbers[a].ToString() : "_";

        while (true)
        {
            Console.CursorLeft = 0;
            Console.CursorVisible = false;
            isOk = numbers.All(number => number != null);

            if (format(0) != "_" && format(1) != "_" && (int.Parse("" + numbers[0] + numbers[1]) > 31 || int.Parse("" + numbers[0] + numbers[1]) <= 0))
            {
                isOk = false;
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (format(2) != "_" && format(3) != "_")
            {
                int days = int.Parse("" + numbers[0] + numbers[1]);
                int month = int.Parse("" + numbers[2] + numbers[3]);
                int year = 0;
                if (format(4) != "_" && format(5) != "_" && format(6) != "_" && format(7) != "_")
                {
                    year = int.Parse("" + numbers[4] + numbers[5] + numbers[6] + numbers[7]);
                }
                if (month == 2 && days > 28)
                {
                    if (days > 29 || !(year % 4 == 0 && year % 100 != 0 || year % 400 == 0))
                    {
                        isOk = false;
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                }
                else if (days > 30 + (month + month /8) % 2)
                {
                    isOk = false;
                    Console.ForegroundColor = ConsoleColor.Red;
                }
            }
            Console.Write(format(0) + format(1));
            Console.ResetColor();
            Console.Write(".");
            
            if (format(2) != "_" && format(3) != "_" && (int.Parse("" + numbers[2] + numbers[3]) > 12  || int.Parse("" + numbers[2] + numbers[3]) <= 0))
            {
                isOk = false;
                Console.ForegroundColor = ConsoleColor.Red;
            }
            
            Console.Write(format(2) + format(3));
            Console.ResetColor();
            Console.Write(".");
            Console.Write(format(4) + format(5) + format(6) + format(7));

            
            Console.CursorLeft = index + index / 2 - index / 6 - index / 8;
            Console.CursorVisible = true;
            int input = GetKeyNumber();
            if (input == -1)
            {
                if (index != 0)
                {
                    numbers[index - 1] = null;
                    index--;
                }
            }
            else if (input == 10)
            {
                if (index == numbers.Length && isOk)
                {
                    DateOnly date = DateOnly.ParseExact(string.Join("",numbers), "ddMMyyyy", null);
                    return date;
                }
            }
            else
            {
                if (index != numbers.Length)
                {
                    numbers[index] = input;
                    index++;
                }
            }
            
        }
        

    }

    
    
    
}
