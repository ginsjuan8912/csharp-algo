    
    /*
     * This helper class provide methods and properties to read CSV files
     * 
     */
    public sealed class CsvReader
    {

        /// <summary>
        /// Represents a boolean that tells if the CSV file has headers or not
        /// </summary>
        private readonly bool hasHeaders;

        /// <summary>
        /// Represents the delimiter of this file
        /// </summary>
        private readonly char delimiter;

        /// <summary>
        /// Represents a HashSet of the headers
        /// </summary>
        private readonly HashSet<string> headers;

        /// <summary>
        /// Represents a queue of the lines that were found in the CSV file
        /// </summary>
        private readonly LinkedList<string> lines;

        /// <summary>
        /// Represents a pointer for the current row that was readed from the CSV file
        /// </summary>
        /// 
        private readonly long currentRow;

        /// <summary>
        /// Represents the current node that the CsvReader is pointing
        /// </summary>
        private LinkedListNode<string> cursor;


        /// <summary>
        /// Return a HashSet with all the headers on this CVS file
        /// </summary>
        public HashSet<string> Headers
        {
            get
            {
                if (hasHeaders)
                {
                    return headers;
                }

                return null;
            }
        }

   

        /// <summary>
        /// Initialize a CSVHHelper by reading all the contents from a CSV file and keeping them ready
        /// for iteration when needed
        /// </summary>
        /// <param Name="reader">stream reader where the content is referenced</param>
        /// <param Name="delimeter">delimeter used in the CSV file, by default with set a comma</param>
        /// <param Name="hasHeader">True if the CSV content has header (by default), otherwise set it to False </param>
        /// <exception cref="ArgumentNullException"></exception>
        public CsvReader(StreamReader stream, char delimeter = ',', bool hasHeader = true)
        {
            currentRow = 0;

            delimiter = delimeter;
            hasHeaders = hasHeader;

            this.headers = new HashSet<string>();
            lines = new LinkedList<string>();

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            try
            {
                stream.BaseStream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StringReader(stream.ReadToEnd()))
                {
                    //Read until the end of the file
                    while (reader.Peek() != -1)
                    {
                        var line = reader.ReadLine();

                        //If the current reading line is 0 and has headers then add headers
                        if (currentRow == 0 && hasHeader)
                        {
                            //Add the headers
                            foreach (var column in line.Split(this.delimiter))
                            {
                                //Add the header to a hashMap
                                headers.Add(column);                         
                            }

                            currentRow++;
                        }

                        //add each line in a queue so all lines from CSV could be iterated
                        lines.AddLast(line.Trim());
                        currentRow++;
                    }
                }
                 
            }
            finally
            {
                //set the cursor to the first item of the list
                cursor = lines.First;
                //Close the reader
                stream.Close();
            }

        }


        /// <summary>
        /// Return the number of lines that are present in the CSV file, without headers.
        /// </summary>
        public int CountLines { get => lines.Count; }

        /// <summary>
        /// Return the value contained in the current position of the cursor
        /// </summary>
        public string CurrentLine => cursor.ToString();

        /// <summary>
        /// Check if the CSV cursor has a next line available to read, if true move the cursor to the next line
        /// </summary>
        /// <returns>Returns true if the cursor has a next line to read, otherwise, return false</returns>
        public bool HasNext()
        {
            if (lines == null)
                return false;

            if (lines.Count == 0)
                return false;


            if (cursor.Next == null)
                return false;

            cursor = cursor.Next;
            return true;
        }

        /// <summary>
        /// Check if the CSV cursor has a previous line available to read, if true move the cursor to the previous line
        /// </summary>
        /// <returns>Returns true if the cursor has a previous line to read, otherwise, return false</returns>
        public bool HasPrevious()
        {
            if (lines == null)
                return false;

            if (lines.Count == 0)
                return false;


            if (cursor.Previous == null)
                return false;

            cursor = cursor.Previous;
            return true;
        }

        /// <summary>
        /// Move the cursor to the next line and returns the value without doing any split by delimeter in the content
        /// </summary>
        /// <returns>Returns the next line of the CSV content</returns>
        /// <exception cref="InvalidOperationException">Throws an exception if the content queue is empty or null</exception>
        public IEnumerable<string> NextLine()
        {
            if (lines == null)
                throw new InvalidOperationException("The CSV list is not initialized");

            if (lines.Count == 0)
                throw new InvalidOperationException("There is not content to read");

            cursor = lines.First;

            while (cursor != null)
            {
                yield return cursor.Value;
                cursor = cursor.Next;
            }


        }

        /// <summary>
        /// Move the cursor to the next line and returns the value splitted with the contents by using the delimiter,
        /// contents of this line will be splitted
        /// </summary>
        /// <returns>Returns the next line of the CSV content as an array of strings</returns>
        /// <exception cref="InvalidOperationException">Throws an exception if the content queue is empty or null</exception>
        public IEnumerable<string[]> NextLineSplitted()
        {
            if (lines == null)
                throw new InvalidOperationException("The CSV list is not initialized");

            if (lines.Count == 0)
                throw new InvalidOperationException("There is not content to read");

            cursor = lines.First;

            while (cursor != null)
            {
                yield return cursor.Value.Split(delimiter);
                cursor = cursor.Next;
            }

        }


        /// <summary>
        /// Move the cursor to the previous line and returns the value without doing any split by delimeter in the content
        /// </summary>
        /// <returns>Returns the previous line of the CSV content</returns>
        /// <exception cref="InvalidOperationException">Throws an exception if the content queue is empty or null</exception>
        public IEnumerable<string> PreviousLine()
        {
            if (lines == null)
                throw new InvalidOperationException("The CSV list is not initialized");

            if (lines.Count == 0)
                throw new InvalidOperationException("There is not content to read");

            cursor = cursor.Previous;

            while (cursor != null)
            {
                yield return cursor.Value;
                cursor = cursor.Previous;
            }
        }

    }