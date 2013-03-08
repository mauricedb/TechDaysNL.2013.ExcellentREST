using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData.Query;
using BasicWebApi.Models;

namespace BasicWebApi.Api
{
    public class BookController : ApiController
    {
        private IBooksRepository _repo = new BooksRepository();

        // GET /api/books to load collection
        // GET /api/books/?$filter=Author eq 'Douglas Adams'
        [Queryable(PageSize = 2)]
        public IQueryable<Book> GetBook()
        {
            var books = _repo.GetBooks().AsQueryable();
            return books;
        }

        // GET /api/books/1 to load single
        public HttpResponseMessage GetBook(int id)
        {
            var book = _repo.GetBook(id);

            if (book == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, book);
            response.Headers.CacheControl = new CacheControlHeaderValue
            {
                MaxAge = TimeSpan.FromSeconds(15)
            };
            return response;
        }

        // PUT /api/books/1 to replace existing
        public Book PutBook(int id, Book book)
        {
            book.Id = id;
            return _repo.UpdateBook(book);
        }

        // POST /api/books to add new 
        public HttpResponseMessage Post(Book book)
        {
            var newBook = _repo.AddBook(book);
            var response = Request.CreateResponse(HttpStatusCode.Created, newBook);
            var uriString = Url.Link("DefaultApi", new { id = newBook.Id });
            response.Headers.Location = new Uri(uriString);
            return response;
        }

        public void DeleteBook(int id)
        {
            _repo.DeleteBook(id);
        }

        public string Options(int id)
        {
            return "Dit zijn de opties";
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && _repo != null)
            {
                _repo.Dispose();
                _repo = null;
            }
        }
    }
}
