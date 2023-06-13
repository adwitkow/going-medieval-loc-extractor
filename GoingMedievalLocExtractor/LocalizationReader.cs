using System.Text;

namespace GoingMedievalLocExtractor
{
    internal class LocalizationReader
    {
        private static readonly string termProperty = "- Term:";
        private static readonly int termLength = termProperty.Length;

        public Dictionary<string, string> Transform(string path)
        {
            var lines = new Dictionary<string, string>();

            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (TryReadTerm(line, out var term))
                    {
                        sr.ReadLine();
                        sr.ReadLine();

                        var value = ReadValue(sr);
                        lines.Add(term!, value);
                    }
                }
            }

            return lines;
        }

        private bool TryReadTerm(string line, out string? currentTerm)
        {
            var chars = new char[termLength];
            var indentation = true;
            var charCount = 0;
            var index = 0;
            foreach (var ch in line)
            {
                index++;
                if (charCount == termLength)
                {
                    break;
                }

                if (indentation && ch == ' ')
                {
                    continue;
                }
                indentation = false;
                chars[charCount] = ch;
                charCount++;
            }

            var currentValue = new string(chars);
            bool result;
            if (currentValue.Equals(termProperty, StringComparison.OrdinalIgnoreCase))
            {
                result = true;
                currentTerm = line.Substring(index).ToLower();
            }
            else
            {
                result = false;
                currentTerm = null;
            }

            return result;
        }

        private string ReadValue(StreamReader sr)
        {
            var builder = new StringBuilder();
            var readingValue = false;
            var finished = false;
            string? line;
            while ((line = sr.ReadLine()) is not null)
            {
                // Naive approach but it seems to work well

                var index = 0;
                foreach (var ch in line)
                {
                    index++;
                    if (ch == '-')
                    {
                        if (readingValue)
                        {
                            finished = true;
                        }
                        else
                        {
                            readingValue = true;
                        }
                        break;
                    }

                    if (ch != ' ')
                    {
                        break;
                    }
                }

                if (finished)
                {
                    break;
                }

                var cutLine = line.Substring(index).Replace("''", "'");
                builder.AppendLine(cutLine);
            }

            return builder.ToString()
                .Trim()
                .TrimStart('\'')
                .TrimEnd('\'');
        }
    }
}
