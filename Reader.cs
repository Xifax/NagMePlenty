using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NagMePlenty
{
    class Reader
    {
        private List<String> rawItems;
        private List<List<String>> parsedItems;
        private Random random;
        private static Logger logger = LogManager.GetCurrentClassLogger();

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

        // Get either random item from all loaded files or empty item.
        public List<String> getRandomItem()
        {
            try {
                int index = random.Next(parsedItems.Count);
                return parsedItems[index];

            // Return 'empty' item, if item list is empty
            } catch (ArgumentOutOfRangeException) {
                List <String> empty = new List<String>();
                empty.AddRange(new String[]{
                    "Error!",
                    "Could not load any items.",
                    "", ""});
                return empty;
            }
        }

        /* Loaders */

        // Load specified file (if exists).
        public async void LoadFile(String file)
        {
            try
            {
                using (StreamReader reader = new StreamReader(Path.GetFullPath(file)))
                {
                    // Read single line
                    String line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        // Save raw line (could be useful)
                        rawItems.Add(line);

                        // Parse line by tabs
                        List<String> subitems = line.Split('\t').ToList();
                        parsedItems.Add(subitems);
                    }

                }
            }
            catch (Exception e)
            {
                logger.Error("Could not read file {0}: {1}", file, e.Message);
            }
        }

        // Load all files in path (top folder only).
        public void LoadPath(String path = null)
        {
            // Try to read all the text files in current folder
            path = (path != null) ? path : ".";
            string[] files = Directory.GetFiles(path, "*.txt", SearchOption.TopDirectoryOnly);

            // Stop if no txt files found
            if (files.Length == 0)
            {
                logger.Info("No txt files found in {0}", path);
                return;
            }

            // Parse those files
            foreach (String file in files)
            {
                LoadFile(file);
            }

        }
    }
}
