namespace MovieLibrary.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        private int recordsPerPage = 10;
        private readonly int RecordsPerPageMax = 50;
        public int RecordsPerPage
        {
            get
            {
                return recordsPerPage;
               }
            set
            {
                if (value > recordsPerPage)
                {
                    recordsPerPage = RecordsPerPageMax;
                }
                else
                {
                    recordsPerPage = value;
                }
            }
        }
    }
}
