//using Invify.Domain.Entities;
//using Invify.Dtos;
//using Invify.Infrastructure.Configuration;
//using Invify.Interfaces;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Invify.Infrastructure.Repositories
//{
//    public class InventoryRepository : RepositoryBase<Inventory>, IInventoryRepository
//    {
//        public InventoryRepository(ApplicationDbContext applicationDbContext)
//        : base(applicationDbContext)
//        {
//        }

//        public async Task<IEnumerable<Inventory>> FindAllAsync()
//        {
//            return await _context.Set<Inventory>().Include(p => p.Product).ToListAsync();
//        }

//        public async Task<Response> CreateAsync(Inventory inventory)
//        {
//            try
//            {
//                if (inventory.ProductId != 0)
//                {
//                    var product = await _context.Set<Product>().FindAsync(inventory.ProductId);
//                    inventory.Product = product;
//                }
//                await _context.Set<Inventory>().AddAsync(inventory);
//                await _context.SaveChangesAsync();
//                return new Response { Success = true, Message = inventory.GetType().Name + " created successfully" };
//            }
//            catch (Exception ex)
//            {
//                return new Response { Success = false, Message = ex.Message };
//            }

//        }

//        public async Task<Response> UpdateAsync(Inventory inventory)
//        {
//            try
//            {
//                if (inventory.ProductId != 0)
//                {
//                    var product = await _context.Set<Product>().FindAsync(inventory.ProductId);
//                    inventory.Product = product;
//                }
//                _context.Attach(inventory);
//                _context.Entry(inventory).State = EntityState.Modified;
//                //_context.Set<T>().Update(entity);
//                await _context.SaveChangesAsync();

//                return new Response { Success = true, Message = inventory.GetType().Name + " updated successfully" };
//            }
//            catch (Exception ex)
//            {
//                return new Response { Success = false, Message = ex.Message };
//            }
//        }
//    }
//}
