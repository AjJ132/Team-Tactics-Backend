using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamTacticsBackend.Database;
using TeamTacticsBackend.DTO.Messages;
using TeamTacticsBackend.Models.Messages;

namespace TeamTacticsBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly UserManager<IdentityUser> usermanager;
        private readonly IDbContextFactory<TeamTacticsDBContext> contextFactory;

        public MessagesController(UserManager<IdentityUser> UserManager, IDbContextFactory<TeamTacticsDBContext> contextFactory)
        {
            this.usermanager = UserManager;
            this.contextFactory = contextFactory;
        }

        //Create new conversation
        [HttpPost("create-conversation")]
        [Authorize]
        public async Task<IActionResult> CreateConversation([FromBody] NewConversationDTO newConversationDTO)
        {
            try
            {
                //verify user
                var identityUser = await usermanager.GetUserAsync(User);

                if (identityUser == null)
                {
                    return StatusCode(401, "User not found");
                }

                //verify conversation name
                if(newConversationDTO.isGroup && string.IsNullOrEmpty(newConversationDTO.conversationName))
                {
                    return StatusCode(400, "Conversation name is required");
                }

                //verify conversation users
                if (newConversationDTO.userIds.Count == 0)
                {
                    return StatusCode(400, "At least one user is required");
                }


                //current user id is not included in the list of users

                Conversation newConversation = new Conversation
                {
                    ConversationId = Guid.NewGuid(),
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = Guid.Parse(identityUser.Id)
                };

                //if is group then add conversation name otherwise conversation name empty
                if(newConversationDTO.isGroup)
                {
                    newConversation.ConversationName = newConversationDTO.conversationName;
                }
                else
                {
                    newConversation.ConversationName = "";
                }

                //create conversation users
                List<ConversationUser> conversationUsers = new List<ConversationUser>();

                foreach (var userId in newConversationDTO.userIds)
                {
                    conversationUsers.Add(new ConversationUser
                    {
                        Index = Guid.NewGuid(),
                        ConversationId = newConversation.ConversationId,
                        UserId = Guid.Parse(userId)
                    });
                }

                using (var context = contextFactory.CreateDbContext())
                {
                    await context.Conversations.AddAsync(newConversation);
                    await context.ConversationUsers.AddRangeAsync(conversationUsers);
                    await context.SaveChangesAsync();


                    //create and return DTOs
                    ReturnConversationDTO returnConversationDTO = new ReturnConversationDTO
                    {
                        conversationId = newConversation.ConversationId,
                        users = new List<ConversationUserDTO>()
                    };

                    //setup conversation users
                    foreach (var conversationUser in conversationUsers)
                    {
                        var user = await context.Users.FindAsync(conversationUser.UserId.ToString());

                        if (user != null)
                        {
                            returnConversationDTO.users.Add(new ConversationUserDTO
                            {
                                userId = conversationUser.UserId,
                                userName = user.FirstName + " " + user.LastName
                            });
                        }
                    }

                    //setup last message and last message time
                    var lastMessage = await context.ConversationMessages.Where(x => x.ConversationId == newConversation.ConversationId).OrderByDescending(x => x.SentAt).FirstOrDefaultAsync();

                    if (lastMessage != null)
                    {
                        returnConversationDTO.lastMessageSent = lastMessage.Message;
                        returnConversationDTO.lastMessageSentTime = lastMessage.SentAt;
                    }
                    else
                    {
                        returnConversationDTO.lastMessageSent = null;
                        returnConversationDTO.lastMessageSentTime = null;
                    }

                    return Ok(returnConversationDTO);
                }


            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("add-conversation-user")]
        [Authorize]
        public async Task<IActionResult> AddConversationUser([FromBody] ConversationUserDTO addConversationUserDTO)
        {
            try
            {
                //verify user
                var identityUser = await usermanager.GetUserAsync(User);

                if (identityUser == null)
                {
                    return StatusCode(401, "User not found");
                }

                //verify conversation
                var conversation = await contextFactory.CreateDbContext().Conversations.FindAsync(addConversationUserDTO.conversationId);

                if (conversation == null)
                {
                    return StatusCode(404, "Conversation not found");
                }

                //verify user
                var user = await contextFactory.CreateDbContext().Users.FindAsync(addConversationUserDTO.userId);

                if (user == null)
                {
                    return StatusCode(404, "User not found");
                }

                //verify user is not already in conversation
                var conversationUser = await contextFactory.CreateDbContext().ConversationUsers.Where(x => x.ConversationId == addConversationUserDTO.conversationId && x.UserId == addConversationUserDTO.userId).FirstOrDefaultAsync();

                if (conversationUser != null)
                {
                    return StatusCode(400, "User already in conversation");
                }

                //ensure conversation id is not null
                if (addConversationUserDTO.conversationId == null)
                {
                    return StatusCode(400, "Conversation id is required");
                }

                //add user to conversation
                await contextFactory.CreateDbContext().ConversationUsers.AddAsync(new ConversationUser
                {
                    Index = Guid.NewGuid(),
                    ConversationId = (Guid)addConversationUserDTO.conversationId,
                    UserId = addConversationUserDTO.userId
                });

                await contextFactory.CreateDbContext().SaveChangesAsync();


                //return conversation users
                List<ConversationUserDTO> conversationUserDTOs = new List<ConversationUserDTO>();

                var conversationUsers = await contextFactory.CreateDbContext().ConversationUsers.Where(x => x.ConversationId == addConversationUserDTO.conversationId).ToListAsync();

                foreach (var cUser in conversationUsers)
                {
                    var thisUser = await contextFactory.CreateDbContext().Users.FindAsync(cUser.UserId.ToString());

                    if (user != null)
                    {
                        conversationUserDTOs.Add(new ConversationUserDTO
                        {
                            userId = cUser.UserId,
                            userName = thisUser.FirstName + " " + thisUser.LastName,
                            conversationId = cUser.ConversationId
                        });
                    }
                }

                return Ok(conversationUserDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("send-message")]
        [Authorize]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDTO newMessage)
        {
            try
            {
                //verify user
                var identityUser = await usermanager.GetUserAsync(User);

                if (identityUser != null)
                {
                    return StatusCode(401, "User not found");
                }

                //verify conversation
                var conversation = await contextFactory.CreateDbContext().Conversations.FindAsync(newMessage.conversationId);

                if (conversation == null)
                {
                    return StatusCode(404, "Conversation not found");
                }

                //verify user is in conversation

                var conversationUser = await contextFactory.CreateDbContext().ConversationUsers.Where(x => x.ConversationId == newMessage.conversationId && x.UserId == Guid.Parse(identityUser.Id)).FirstOrDefaultAsync();

                if (conversationUser == null)
                {
                    return StatusCode(400, "User not in conversation");
                }

                //verify message
                if (string.IsNullOrEmpty(newMessage.message))
                {
                    return StatusCode(400, "Message is required");
                }

                DateTimeOffset utcNow = DateTimeOffset.UtcNow;

                //send message
                await contextFactory.CreateDbContext().ConversationMessages.AddAsync(new ConversationMessage
                {
                    MessageId = Guid.NewGuid(),
                    ConversationId = newMessage.conversationId,
                    SenderId = Guid.Parse(identityUser.Id),
                    Message = newMessage.message,
                    SentAt = utcNow
                });

                await contextFactory.CreateDbContext().SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("conversations")]
        [Authorize]
        public async Task<IActionResult> GetConversations()
        {
            try
            {
                //verify user
                var identityUser = await usermanager.GetUserAsync(User);

                if (identityUser == null)
                {
                    return StatusCode(401, "User not found");
                }

                //get conversations
                var conversations = await contextFactory.CreateDbContext().Conversations.Where(x => x.CreatedBy == Guid.Parse(identityUser.Id)).ToListAsync();

                List<ReturnConversationDTO> returnConversationDTOs = new List<ReturnConversationDTO>();

                foreach (var conversation in conversations)
                {
                    ReturnConversationDTO returnConversationDTO = new ReturnConversationDTO
                    {
                        conversationId = conversation.ConversationId,
                        conversationName = conversation.ConversationName,
                        users = new List<ConversationUserDTO>()
                    };

                    //get conversation users
                    var conversationUsers = await contextFactory.CreateDbContext().ConversationUsers.Where(x => x.ConversationId == conversation.ConversationId).ToListAsync();

                    foreach (var conversationUser in conversationUsers)
                    {
                        var user = await contextFactory.CreateDbContext().Users.FindAsync(conversationUser.UserId.ToString());

                        if (user != null)
                        {
                            returnConversationDTO.users.Add(new ConversationUserDTO
                            {
                                userId = conversationUser.UserId,
                                userName = user.FirstName + " " + user.LastName
                            });
                        }
                    }

                    //get last message
                    var lastMessage = await contextFactory.CreateDbContext().ConversationMessages.Where(x => x.ConversationId == conversation.ConversationId).OrderByDescending(x => x.SentAt).FirstOrDefaultAsync();

                    if (lastMessage != null)
                    {
                        returnConversationDTO.lastMessageSent = lastMessage.Message;
                        returnConversationDTO.lastMessageSentTime = lastMessage.SentAt;
                    }
                    else
                    {
                        returnConversationDTO.lastMessageSent = null;
                        returnConversationDTO.lastMessageSentTime = null;
                    }

                    returnConversationDTOs.Add(returnConversationDTO);
                }

                return Ok(returnConversationDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
