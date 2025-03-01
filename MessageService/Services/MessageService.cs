using System;
using MessageService.Data;
using MessageService.Interfaces;
using MessageService.Interfaces.Services;
using MessageService.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Services;

public class MessageService : IMessageService
{
    private readonly ILogger<MessageService> _logger;
    private readonly IMessageRepository _messageRepository;
    public MessageService(IMessageRepository messageRepository, ILogger<MessageService> logger)
    {
        _messageRepository = messageRepository;
        _logger = logger;
    }

    public async Task<ServiceResult<Message>> CreateMessage(Message message)
    {
        try
        {
            if (message == null)
            {
                _logger.LogWarning("Attempted to create null message");
                return ServiceResult<Message>.BadRequest("Message cannot be null");
            }

            if (string.IsNullOrWhiteSpace(message.Text))
            {
                _logger.LogWarning("Attempted to create message with empty content");
                return ServiceResult<Message>.BadRequest("Message content cannot be empty");
            }

            if (message.ChannelId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to create message with invalid channel ID");
                return ServiceResult<Message>.BadRequest("Channel ID is required");
            }

            message.CreatedAt = DateTime.UtcNow;
            await _messageRepository.AddAsync(message);
            _logger.LogInformation("Message {MessageId} created successfully for channel {ChannelId}",
                message.Id, message.ChannelId);
            return ServiceResult<Message>.Created(message);

        }
        catch (DbUpdateException e)
        {
            _logger.LogError(e, "Database error creating message for channel {ChannelId}", message?.ChannelId);
            return ServiceResult<Message>.Error("A database error occurred while creating the message");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating message for channel {ChannelId}", message?.ChannelId);
            return ServiceResult<Message>.Error("An unexpected error occurred while creating the message");
        }
    }

    public async Task<ServiceResult<bool>> DeleteMessageAsync(Guid messageId)
    {
        try
        {
            if (messageId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to delete message with empty ID");
                return ServiceResult<bool>.BadRequest("Message ID cannot be empty");
            }

            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null)
            {
                _logger.LogError("Message with id {MessageId} not found", messageId);
                return ServiceResult<bool>.NotFound("Message not found");

            }

            await _messageRepository.DeleteAsync(message);
            _logger.LogInformation("Deleted message with id {MessageId}", messageId);
            return ServiceResult<bool>.Success(true, "Message deleted successfully");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error deleting message {MessageId}", messageId);
            return ServiceResult<bool>.Error("A database error occurred while deleting the message");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting message {MessageId}", messageId);
            return ServiceResult<bool>.Error("An unexpected error occurred while deleting the message");
        }


    }

    public async Task<ServiceResult<IEnumerable<Message>>> GetMessagesInChannelAsync(Guid channelId, int limit, int page)
    {
        try
        {
            if (channelId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get messages with empty channel ID");
                return ServiceResult<IEnumerable<Message>>.BadRequest("Channel ID cannot be empty");
            }

            if (limit <= 0) limit = 20;
            if (limit > 100) limit = 100; // Prevent excessive queries
            if (page <= 0) page = 1;

            var messages = await _messageRepository.GetMessagesInChannelAsync(channelId, limit, page);
            _logger.LogInformation("Retrieved {MessageCount} messages for channel {ChannelId}", messages.Count(), channelId);
            return ServiceResult<IEnumerable<Message>>.Success(messages, "Messages retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving messages for channel {ChannelId}", channelId);
            return ServiceResult<IEnumerable<Message>>.Error("An error occurred while retrieving messages");
        }
    }

    public async Task<ServiceResult<Message>> UpdateMessage(Guid messageId, string newContent)
    {
        try
        {
            if (messageId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to update message with empty ID");
                return ServiceResult<Message>.BadRequest("Message ID cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(newContent))
            {
                _logger.LogWarning("Attempted to update message with empty content");
                return ServiceResult<Message>.BadRequest("Message content cannot be empty");
            }

            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null)
            {
                _logger.LogWarning("Message {MessageId} not found for update", messageId);
                return ServiceResult<Message>.NotFound("Message not found");
            }

            message.Text = newContent;
            await _messageRepository.UpdateAsync(message);
             _logger.LogInformation("Message {MessageId} updated successfully", messageId);
            return ServiceResult<Message>.Success(message);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error updating message {MessageId}", messageId);
            return ServiceResult<Message>.Error("A database error occurred while updating the message");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating message {MessageId}", messageId);
            return ServiceResult<Message>.Error("An unexpected error occurred while updating the message");
        }
    }

}
