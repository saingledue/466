#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SEWebApp.Models;
using System.Collections;

namespace SEWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly fundDBContext _context;

        public PurchaseController(fundDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchases()
        {
            return await _context.Purchases.ToListAsync();
        }

        [HttpGet("{userID}")]
        public async Task<IEnumerable<Purchase>> GetPurchases(long userID)
        {
            var purchases = await _context.Purchases.Where(i=>(i.UserId == userID)).ToListAsync();

            if (purchases == null)
            {
                return null;
            }

            return purchases;
        }
        [HttpGet("purchasedList/{userID}")]
        public async Task<IEnumerable<long>> GetPurchasesdItemsList(long userID)
        {
            var purchases = await _context.Purchases.Where(i => (i.UserId == userID)).ToListAsync();

            if (purchases == null)
            {
                return null;
            }
            var stickerIDs = new ArrayList();
            foreach ( var purchase in purchases)
            {
                stickerIDs.Add(purchase.StickerId);
            }
            return stickerIDs.Cast<long>().ToList();
        }
        [HttpGet("{userID}/{stickerID}/{purchaseAmount}")]
        public async Task<ActionResult<Purchase>> PurchaseSticker(long userID, long stickerID, int purchaseAmount)
        {
            Purchase purchase = new Purchase() { PurchaseTime=DateTime.Now,StickerId=stickerID,UserId=userID};
            _context.Purchases.Add(purchase);
            var userToSendTo = await _context.Users.Where(o => (o.Id == userID)).FirstOrDefaultAsync();
            userToSendTo.SpendablePoints = userToSendTo.SpendablePoints - purchaseAmount;
            await _context.SaveChangesAsync();
            return purchase;
        }
        [HttpPost]
        public async Task<ActionResult<Purchase>> PostPurchase(Purchase purchase)
        {
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPurchases), new { id = purchase.Id }, purchase);

        }

        [HttpGet("/getTopPurchases")]
        public async Task<IEnumerable<long>> TopPurchases()
        {
            var purchases = await _context.Purchases.ToListAsync();
            var stickerIDS = new ArrayList();
            foreach (var purchase in purchases)
            {
                stickerIDS.Add(purchase.StickerId);
            }
            IEnumerable<long> stickerList = stickerIDS.Cast<long>().ToList();
            IEnumerable<long> top3 = stickerList.GroupBy(i => i).OrderByDescending(g => g.Count()).Take(3).Select(g => g.Key);
            return top3;
        }

    }
}
