﻿using SheduleManagement.Data.EF;
using SheduleManagement.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SheduleManagement.Data.Services
{
    public class EventUserService
    {
        private readonly ScheduleManagementDbContext _dbContext;
        public EventUserService(ScheduleManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string UpdateForEvent(int eventId, int creatorId, List<int> participants)
        {
            try
            {
                _dbContext.EventUsers.RemoveRange(_dbContext.EventUsers.Where(x => !participants.Contains(x.UserId) && x.EventId == eventId));
                var existsParticipants = _dbContext.EventUsers.Where(x => !participants.Contains(x.UserId) && x.EventId == eventId).Select(x => x.UserId).ToList();
                _dbContext.EventUsers.AddRange(participants.Where(x => !existsParticipants.Contains(x)).Select(x => new EventUser
                {
                    EventId = eventId,
                    UserId = x,
                    Status = (int)EventUserStatus.Invited,
                    LastUpdate = DateTime.Now
                }));
                var creator = _dbContext.EventUsers.FirstOrDefault(x => x.UserId == creatorId && x.EventId == eventId);
                if (creator == null)
                {
                    _dbContext.EventUsers.Add(new EventUser
                    {
                        EventId = eventId,
                        UserId = creatorId,
                        Status = (int)EventUserStatus.Accepted,
                        LastUpdate = DateTime.Now
                    });
                }
                var a = _dbContext.EventUsers.ToList();
                _dbContext.SaveChanges();
                return String.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string DeleteForEvent(int eventId)
        {
            try
            {
                _dbContext.EventUsers.RemoveRange(_dbContext.EventUsers.Where(x => x.EventId == eventId));
                _dbContext.SaveChanges();
                return String.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string Delete(int eventId, int userId)
        {
            try
            {
                var eventUser = _dbContext.EventUsers.Where(x => x.UserId == userId && x.EventId == eventId).FirstOrDefault();
                if (eventUser == null) return "Không tìm tháy lời mời tương ứng.";
                _dbContext.EventUsers.Remove(eventUser);
                _dbContext.SaveChanges();
                return String.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
