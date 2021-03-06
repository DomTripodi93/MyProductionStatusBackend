using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.Helpers;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data
{
    public class ManagementRepository : IManagementRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        
        public ManagementRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Mach> GetMachine(int userId, string mach)
        {
            var machine = await _context.Machines
                .Where(m => m.userId == userId)
                .FirstOrDefaultAsync(m => m.Machine == mach);
            return machine;
        }

        public async Task<IEnumerable<Mach>> GetAllMachines(int userId)
        {
            var machines = await _context.Machines
                .Where(m => m.userId == userId)
                .ToListAsync();
                
            return machines;
        }

        public async Task<IEnumerable<Mach>> GetMachines(int userId, string machType)
        {
            var machines = await _context.Machines
                .Where(m => m.userId == userId)
                .Where(m => m.MachType == machType)
                .ToListAsync();
                
            return machines.OrderBy(m => m.Machine);
        }

        public async Task<IEnumerable<Mach>> GetMachinesByJob(int userId)
        {
            var user = await _context.Users
                .Include(x => x.Machine)
                .FirstOrDefaultAsync(u => u.Id == userId);
                
            return user.Machine.OrderByDescending(m => m.CurrentJob);
        }

        public async Task<Part> GetPart(int userId, string part)
        {
            var partToReturn = await _context.Parts
                .Where(p => p.userId == userId)
                .FirstOrDefaultAsync(p => p.PartNumber == part);

            return partToReturn;
        }

        public async Task<IEnumerable<Part>> GetPartsByNumber(int userId, string part)
        {
            var partsToReturn = await _context.Parts
                .Where(p => p.userId == userId)
                .Where(p => p.PartNumber.Contains(part))
                .ToListAsync();

            return partsToReturn;
        }

        public  async Task<IEnumerable<Part>> GetAnyParts(int userId)
        {
            var parts = await _context.Parts
                .Where(p => p.userId == userId)
                .ToListAsync();
                
            return parts;
        }

        public  async Task<IEnumerable<Part>> GetParts(int userId, string machType)
        {
            var parts = await _context.Parts
                .Where(p => p.userId == userId)
                .Where(p => p.MachType == machType)
                .Where(p => p.Active == "Active")
                .ToListAsync();
                
            return parts;
        }

        public  async Task<IEnumerable<Part>> GetAllParts(int userId, string machType)
        {
            var parts = await _context.Parts
                .Where(p => p.userId == userId)
                .Where(p => p.MachType == machType)
                .ToListAsync();
                
            return parts;
        }

        public async Task<Part> GetPartByJob(int userId, string jobNum)
        {
            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.JobNumber == jobNum);

            var parts = await _context.Parts
                .FirstOrDefaultAsync(p => p.PartNumber == job.PartNumber);
                
            return parts;
        }

        public async Task<Job> GetJob(int userId, string jobNum)
        {

            var job = await _context.Jobs        
                .Where(j => j.userId == userId)
                .FirstOrDefaultAsync(j => j.JobNumber == jobNum);

            return job;
        }

        public async Task<IEnumerable<Job>> GetAnyJobs(int userId)
        {

            var job = await _context.Jobs
                .Where(j => j.Active == "Active")
                .Where(j => j.userId == userId)
                .ToListAsync();

            return job;
        }

        public async Task<PagedList<Job>> GetJobs(int userId, PagingParams jobParams, string machType)
        {
            var jobs = _context.Jobs
                .Where(j => j.userId == userId)
                .Where(j => j.Active == "Active")
                .Where(j => j.MachType == machType);

            var jobsForReturn = jobs.OrderByDescending(j => j.JobNumber);

            return await PagedList<Job>.CreateAsync(jobsForReturn, jobParams.PageNumber, jobParams.PageSize);
        }

        public async Task<PagedList<Job>> GetJobsByDate(int userId, PagingParams jobParams, string machType)
        {
            var jobs = _context.Jobs
                .Where(j => j.userId == userId)
                .Where(j => j.Active == "Active")
                .Where(j => j.MachType == machType);

            var jobsForReturn = jobs.OrderBy(j => j.DeliveryDate);

            return await PagedList<Job>.CreateAsync(jobsForReturn, jobParams.PageNumber, jobParams.PageSize);
        }

        public async Task<PagedList<Job>> GetAllJobsByType(int userId, PagingParams jobParams, string machType)
        {
            var jobs = _context.Jobs
                .Where(j => j.userId == userId)
                .Where(j => j.MachType == machType);

            var jobsForReturn = jobs.OrderByDescending(j => j.JobNumber);

            return await PagedList<Job>.CreateAsync(jobsForReturn, jobParams.PageNumber, jobParams.PageSize);
        }

        public async Task<IEnumerable<Job>> GetJobsByPart(int userId, string partNumber)
        {
            var jobs = await _context.Jobs
                .Where(j => j.userId == userId)
                .Where(p => p.PartNumber == partNumber)
                .ToListAsync();

            return jobs;
        }

        public async Task<Operation> GetOp(int userId, string jobNum, string opNum)
        {
            var operation = await _context.Operations
                .Include(x => x.Production)
                .Where(m => m.userId == userId)
                .Where(o => o.JobNumber == jobNum)
                .FirstOrDefaultAsync(o => o.OpNumber == opNum);

            return operation;
        }

        public async Task<IEnumerable<Operation>> GetOpsByJob(int userId, string jobNum)
        {
            var operations = await _context.Operations
                .Include(x => x.Production)
                .Where(m => m.userId == userId)
                .Where(o => o.JobNumber == jobNum)
                .ToListAsync();

            return operations;
        }

        public async Task<IEnumerable<Operation>> GetOpsByMach(int userId, string jobNum, string mach)
        {
            var operations = await _context.Operations
                .Where(m => m.userId == userId)
                .Where(o => o.JobNumber == jobNum)
                .Where(o => o.Machine == mach)
                .ToListAsync();

            return operations;
        }

        public async Task<Production> GetProduction(int id)
        {
            var production = await _context.Production.FirstOrDefaultAsync(p => p.Id == id);
            
            return production;
        }

        public async Task<IEnumerable<Production>> GetAnyProduction(int userId)
        {
            var prodForReturn = await _context.Production
                .Where(o => o.userId == userId)
                .ToListAsync();

            return prodForReturn;
        }

        public async Task<PagedList<Production>> GetProductionSet(int userId, PagingParams prodParams, string machType)
        {
            var prod = _context.Production
                .Where(j => j.MachType == machType)
                .Where(p => p.userId == userId);
            
            var prodForReturn = prod.OrderByDescending(p => p.Date);

            return await PagedList<Production>.CreateAsync(prodForReturn, prodParams.PageNumber, prodParams.PageSize);
        }

        public async Task<IEnumerable<Production>> GetProductionSetByJob(int userId, string job)
        {
            var prodForReturn = await _context.Production
                .Where(o => o.userId == userId)
                .Where(o => o.JobNumber == job)
                .ToListAsync();

            return prodForReturn.OrderByDescending(p => p.Date);
        }

        public async Task<IEnumerable<Production>> GetProductionSetByOp(int userId, string job, string op)
        {
            var prodForReturn = await _context.Production
                .Where(o => o.userId == userId)
                .Where(o => o.JobNumber == job)
                .Where(o => o.OpNumber == op)
                .ToListAsync();

            return prodForReturn.OrderByDescending(p => p.Date);
        }

        public async Task<IEnumerable<Production>> GetProductionSetByJobOpAndMachine(int userId, string job, string op, string mach)
        {
            var prodForReturn = await _context.Production
                .Where(j => j.userId == userId)
                .Where(p => p.JobNumber == job)
                .Where(p => p.OpNumber == op)
                .Where(p => p.Machine == mach)
                .ToListAsync();

            return prodForReturn.OrderBy(p => p.Shift).OrderBy(p => p.Date);
        }

        public async Task<IEnumerable<Production>> GetProductionSetByDate(int userId, string date)
        {
            DateTime DateAsDate = DateTime.Parse(date);

            var prodForReturn = await _context.Production
                .Where(p => p.Date == DateAsDate)
                .Where(m => m.userId == userId)
                .ToListAsync();

            return prodForReturn.OrderByDescending(p => p.Date);
        }

        public async Task<IEnumerable<Production>> GetProductionSetByMachineAndDate(int userId, string date, string op, string job, string mach)
        {
            DateTime DateAsDate = DateTime.Parse(date);

            var prodForReturn = await _context.Production
                .Where(p => p.userId == userId)
                .Where(p => p.JobNumber == job)
                .Where(p => p.OpNumber == op)
                .Where(p => p.Machine == mach)
                .Where(p => p.Date == DateAsDate)
                .Where(p => p.Shift != "Found")
                .ToListAsync();
                
            return prodForReturn.OrderBy(p => p.Shift);
        }

        public async Task<IEnumerable<Production>> GetProductionFoundByMachineAndDate(int userId, string date, string op, string job, string mach)
        {
            DateTime DateAsDate = DateTime.Parse(date);

            var prodFound = await _context.Production
                .Where(p => p.userId == userId)
                .Where(p => p.Shift == "Found")
                .Where(p => p.JobNumber == job)
                .Where(p => p.OpNumber == op)
                .Where(p => p.Machine == mach)
                .Where(p => p.Date == DateAsDate)
                .ToListAsync();

                
            return prodFound;
        }

        public async Task<Hourly> GetAnyHourly(int userId)
        {
            var hourly = await _context.Hourlys
            .FirstOrDefaultAsync(p => p.userId == userId);
            
            return hourly;
        }

        public async Task<Hourly> GetHourly(int id)
        {
            var hourly = await _context.Hourlys
            .FirstOrDefaultAsync(p => p.Id == id);
            
            return hourly;
        }

        public async Task<IEnumerable<Hourly>> GetHourlySetByDateAndMachine(int userId, string date, string mach)
        {

            DateTime dateAsDate = DateTime.Parse(date);

            var hourlyForReturn = await _context.Hourlys
                .Where(j => j.userId == userId)
                .Where(p => p.Date.Date == dateAsDate.Date)
                .Where(p => p.Machine == mach)
                .ToListAsync();

            return hourlyForReturn.OrderBy(o => o.Time);
        }

        public async Task<IEnumerable<Hourly>> GetHourlySetByDateMachineJobAndOp(int userId, string date, string mach, string job, string op)
        {

            DateTime dateAsDate = DateTime.Parse(date);

            var hourlyForReturn = await _context.Hourlys
                .Where(j => j.userId == userId)
                .Where(p => p.Date.Date == dateAsDate.Date)
                .Where(p => p.Machine == mach)
                .Where(h => h.JobNumber == job)
                .Where(h => h.OpNumber == op)
                .ToListAsync();

            return hourlyForReturn.OrderBy(o => o.Time);
        }

    }
}