using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using SignalRDemo.Hub;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IHubContext<MessageHub, IMessageHubClient> _messageHub;
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration, IHubContext<MessageHub, IMessageHubClient> messageHub)
        {
            _messageHub = messageHub;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AddNewUser")]
        public async Task<ActionResult<USER>> InsertUser([FromBody] USER user)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    if (user.ID == -1)
                    {
                        using (SqlCommand command = new SqlCommand(
                            "INSERT INTO [TBL_USERS] (FNAME, LNAME, EMAIL) VALUES (@FNAME, @LNAME, @EMAIL); SELECT SCOPE_IDENTITY()",
                            connection))
                        {
                            command.Parameters.AddWithValue("@FNAME", user.FNAME);
                            command.Parameters.AddWithValue("@LNAME", user.LNAME);
                            command.Parameters.AddWithValue("@EMAIL", user.EMAIL);

                            int newUserId = Convert.ToInt32(await command.ExecuteScalarAsync());
                            user.ID = newUserId;

                            var hubContext = _messageHub.Clients.All;
                            await hubContext.SendOffersToUser(user);

                            return Ok(user);
                        }
                    }
                    else
                    {
                        using (SqlCommand command = new SqlCommand(
                            "UPDATE [TBL_USERS] SET FNAME = @FNAME, LNAME = @LNAME, EMAIL = @EMAIL WHERE ID = @ID",
                            connection))
                        {
                            command.Parameters.AddWithValue("@FNAME", user.FNAME);
                            command.Parameters.AddWithValue("@LNAME", user.LNAME);
                            command.Parameters.AddWithValue("@EMAIL", user.EMAIL);
                            command.Parameters.AddWithValue("@ID", user.ID);

                            await command.ExecuteNonQueryAsync();

                            var hubContext = _messageHub.Clients.All;
                            await hubContext.UpdateOffersToUser(user);

                            return Ok(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database Error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<ActionResult<USER>> DeleteUser(USER user)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        "DELETE FROM [TBL_USERS] WHERE ID = @ID",
                        connection))
                    {
                        command.Parameters.AddWithValue("@ID", user.ID);

                        // ExecuteNonQueryAsync is used for operations that do not return a result set
                        await command.ExecuteNonQueryAsync();

                        var hubContext = _messageHub.Clients.All;
                        await hubContext.RemoveOffersToUser(user);

                        return Ok();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database Error: {ex.Message}");
            }
        }
    }
}
