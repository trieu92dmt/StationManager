using ISD.Constant;
using ISD.EntityModels;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ISD.Repositories
{
    public class ConstructionRepository
    {
        private EntityDataContext _context;

        /// <summary>
        /// Hảm khởi tạo
        /// </summary>
        /// <param name="db">Truyền vào Db Context</param>
        public ConstructionRepository(EntityDataContext db)
        {
            _context = db;
        }

        #region Danh sách đơn vị thi công An Cường của dự án
        public List<ConstructionViewModel> GetInternalConstruction(Guid? ProfileId)
        {
            var result = new List<ConstructionViewModel>();

            if (ProfileId.HasValue)
            {
                result = (from p in _context.Profile_Opportunity_InternalModel
                          join c in _context.ProfileModel on p.InternalId equals c.ProfileId
                          where p.ProfileId == ProfileId
                          select new ConstructionViewModel()
                          {
                              OpportunityConstructionId = p.OpportunityInternalId,
                              ConstructionId = p.InternalId,
                              ConstructionName = c.ProfileName,
                          }).ToList();
            }

            return result;
        }
        #endregion

        #region Danh sách đơn vị thi công Đối thủ của dự án
        public List<ConstructionViewModel> GetCompetitorConstruction(Guid? ProfileId)
        {
            var result = new List<ConstructionViewModel>();

            if (ProfileId.HasValue)
            {
                result = (from p in _context.Profile_Opportunity_CompetitorModel
                          join c in _context.ProfileModel on p.CompetitorId equals c.ProfileId
                          where p.ProfileId == ProfileId
                          select new ConstructionViewModel()
                          {
                              OpportunityConstructionId = p.OpportunityCompetitorId,
                              ConstructionId = p.CompetitorId,
                              ConstructionName = c.ProfileName,
                          }).ToList();
            }

            return result;
        }
        #endregion
    }
}
