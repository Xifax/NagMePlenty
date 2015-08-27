using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace NagMePlenty
{
    class Reader
    {
        private List<String> rawItems;
        private List<List<String>> parsedItems;
        private Random random;

        public Reader()
        {
            rawItems = new List<String>();
            parsedItems = new List<List<String>>();
            random = new Random();
        }

        /* Getters */
        public List<List<String>> getParsedItems()
        {
            return parsedItems;
        }

        public List<String> getRandomItem()
        {
            try {
                int index = random.Next(parsedItems.Count);
                return parsedItems[index];

            // Return 'empty' item, if item list is empty
            } catch (ArgumentOutOfRangeException) {
                List <String> empty = new List<String>();
                empty.Add("Error!");
                empty.Add("Could not load any items.");
                empty.Add("");
                empty.Add("");
                return empty;
            }
        }

        /* Loader */
        public async void LoadFile(String path = null)
        {
            // Try to read all the text files in current folder
            if (path == null)
            {
                path = ".";
            }
            string[] files = Directory.GetFiles(path, "*.txt", SearchOption.TopDirectoryOnly);

            if (files.Length == 0)
            {
                return;
            }

            // Parse those files
            foreach (String file in files)
            {
                try
                {
                    //using (StreamReader reader = new StreamReader(Path.GetFullPath("Resources/jglossator-export.txt")))
                    using (StreamReader reader = new StreamReader(Path.GetFullPath(file)))
                    {
                        // Read single line
                        String line;
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            rawItems.Add(line);

                            // Parse line by tabs
                            List<String> subitems = line.Split('\t').ToList();
                            parsedItems.Add(subitems);
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }

        }
    }
}
