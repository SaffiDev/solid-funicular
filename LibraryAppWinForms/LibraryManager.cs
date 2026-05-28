using System;
using System.Collections.Generic;
using System.Configuration; 
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace LibraryAppWinForms
{
    public class LibraryManager
    {
        private readonly string _connectionString = 
            ConfigurationManager.ConnectionStrings["LibraryDB"].ConnectionString;

        /// <summary>
        /// Возвращает список всех книг из базы данных.
        /// </summary>
        public async Task<List<Book>> GetAllBooks()
        {
            var books = new List<Book>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand("SELECT Id, Title, Author, Year FROM Books", conn);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        books.Add(new Book
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Author = reader.GetString(2),
                            Year = reader.GetInt32(3)
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка при загрузке книг: " + ex.Message);
            }
            return books;
        }

        /// <summary>
        /// Добавляет книгу в базу данных.
        /// </summary>
        public async Task AddBook(Book book)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(
                        "INSERT INTO Books (Title, Author, Year) VALUES (@Title, @Author, @Year)", conn);
                    cmd.Parameters.AddWithValue("@Title", book.Title);
                    cmd.Parameters.AddWithValue("@Author", book.Author);
                    cmd.Parameters.AddWithValue("@Year", book.Year);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка при добавлении книги: " + ex.Message);
            }
        }

        /// <summary>
        /// Удаляет книгу по идентификатору.
        /// </summary>
        public async Task DeleteBook(int id)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand("DELETE FROM Books WHERE Id = @Id", conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка при удалении книги: " + ex.Message);
            }
        }

        /// <summary>
        /// Поиск книг по названию (частичное совпадение, без учёта регистра).
        /// </summary>
        public async Task<List<Book>> SearchByTitle(string part)
        {
            return await Search("Title", part);
        }

        /// <summary>
        /// Поиск книг по автору (частичное совпадение, без учёта регистра).
        /// </summary>
        public async Task<List<Book>> SearchByAuthor(string part)
        {
            return await Search("Author", part);
        }

        private async Task<List<Book>> Search(string field, string part)
        {
            var result = new List<Book>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(
                        $"SELECT Id, Title, Author, Year FROM Books WHERE {field} LIKE @Part COLLATE Cyrillic_General_CI_AI", conn);
                    cmd.Parameters.AddWithValue("@Part", "%" + part + "%");
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        result.Add(new Book
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Author = reader.GetString(2),
                            Year = reader.GetInt32(3)
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Ошибка при поиске: " + ex.Message);
            }
            return result;
        }
    }
}