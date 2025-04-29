using AutoMapper;
using Castle.Components.DictionaryAdapter.Xml;
using Castle.Core.Logging;
using DotNetEnv;
using Foodify_DoAn.Data;
using Foodify_DoAn.Model;
using Foodify_DoAn.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Npgsql.PostgresTypes;
using Npgsql.Replication;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;

namespace Foodify_DoAn.Service
{
    public class RecipeService : IRecipeRepository
    {
        private readonly FoodifyContext _context;
        private readonly IMapper _dbMapper;
        private readonly IAccountRepository _account;


        public RecipeService(FoodifyContext context, IMapper dbMapper, IAccountRepository accountRepository)
        {
            _context = context;
            _dbMapper = dbMapper;
            _account = accountRepository;
        }

        public async Task<RecipeResultDto> addCongThuc( RecipeDto congthuc)
        {
            if (string.IsNullOrEmpty(congthuc.token)) return null;
            var user = await _account.AuthenticationAsync(new TokenModel { AccessToken = congthuc.token });
            if (user == null) return null;

            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaTK == user.Id);
            if (nguoiDung == null) return null; 


            CongThuc recipe = new CongThuc();
             _dbMapper.Map(congthuc, recipe);
            recipe.MaND = nguoiDung.MaND;
            recipe.NguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaND == nguoiDung.MaND);
            await _context.CongThucs.AddAsync(recipe);
            await _context.SaveChangesAsync();

            List<NguyenLieuDto> nguyenLieuList = new List<NguyenLieuDto>();

            foreach (var nl in congthuc.NguyenLieus)
            {
                var CtCongThuc = new CTCongThuc
                {
                    MaCT = recipe.MaCT,
                    MaNL = nl.MaNL,
                    DinhLuong = nl.DinhLuong,
                    DonViTinh = nl.DonViTinh
                };
                await _context.CTCongThucs.AddAsync(CtCongThuc);

                nguyenLieuList.Add(new NguyenLieuDto
                {
                    MaNL = nl.MaNL,
                    DinhLuong = nl.DinhLuong,
                    DonViTinh = nl.DonViTinh
                });
            }

            await _context.SaveChangesAsync();

            var result = new RecipeResultDto
            {
                MaCT = recipe.MaCT,
                TenCT = recipe.TenCT,
                NguyenLieus = nguyenLieuList
            };

            return result;
        }

   

        public async Task<bool> deleteCongThuc(int id)
        {
            var congthuc = await _context.CongThucs.FindAsync(id);
            if (congthuc == null) return false;
            _context.Remove(congthuc);
            await _context.SaveChangesAsync();
            return true; 
        }

        public async Task<List<PostResultDto>> getAllCongThucs(RecipeRequestDto recipe)
        {
            if (recipe == null)
                throw new ArgumentNullException(nameof(recipe));

            int skip = (recipe.PageNumber - 1) * recipe.PageSize;
            int take = recipe.PageSize;

            // Authenticate user once
            var user = string.IsNullOrEmpty(recipe.Token)
                ? null
                : await _account.AuthenticationAsync(new TokenModel { AccessToken = recipe.Token });

            List<int> followingUserIds = new();

            if (user != null)
            {
                followingUserIds = await _context.TheoDois
                    .Where(x => x.Following_ID == user.Id)
                    .Select(a => a.Followed_ID)
                    .ToListAsync();
            }

            // Query recipes along with their authors
            var recipeDtos = await _context.CongThucs
                .Join(_context.NguoiDungs,
                    ct => ct.MaND,
                    nd => nd.MaND,
                    (ct, nd) => new { CongThuc = ct, TacGia = nd })
                .OrderByDescending(x => followingUserIds.Contains(x.CongThuc.MaND))
                .ThenByDescending(x => x.CongThuc.NgayCapNhat)
                .Skip(skip)
                .Take(take)
                .Select(x => new PostResultDto
                {
                    TenCT = x.CongThuc.TenCT,
                    MoTaCT = x.CongThuc.MoTaCT,
                    TongCalories = x.CongThuc.TongCalories,
                    AnhCT = x.CongThuc.AnhCT,
                    LuotXem = x.CongThuc.LuotXem,
                    LuotLuu = x.CongThuc.LuotLuu,
                    LuotThich = x.CongThuc.LuotThich,
                    TacGia = new NguoiDungDto
                    {
                       
                        MaTK = x.TacGia.MaTK,
                        TenND = x.TacGia.TenND,
                        GioiTinh = x.TacGia.GioiTinh,
                        NgaySinh = x.TacGia.NgaySinh,
                        TieuSu = x.TacGia.TieuSu,
                        SDT = x.TacGia.SDT,
                        Email = x.TacGia.Email,
                        DiaChi = x.TacGia.DiaChi,
                        LuotTheoDoi = x.TacGia.LuotTheoDoi,
                        AnhDaiDien = x.TacGia.AnhDaiDien
                    }
                })
                .ToListAsync();

            return recipeDtos;
        }


        public async Task<CongThuc> getByID(int id)
        {
            var recipe = await _context.CongThucs.FindAsync(id);
            if (recipe == null) return null!;
            return recipe;
        }

        public async Task<PostAndCommentsDto> GetOnePostInDetail(Like_Share_GetOnePostDto request)
        {
            var account = await _account.AuthenticationAsync(new TokenModel { AccessToken = request.token });

            if (account == null) return null!;

            var post = await _context.CongThucs.FirstOrDefaultAsync(x => x.MaCT == request.IdCongThuc);

            if (post == null) return null!;


            var user = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaTK == account.Id);
            if (user == null) return null!;

            var listUserComment = await _context.Comments
                .Where(x => x.MaBaiViet == post.MaCT)
                .Select(x => new { x.MaND, x.ThoiGian, x.NoiDung }) // Use 'new' keyword to create an anonymous object
                .ToListAsync();
            List<CommentResultDto> commentResultDtos = new List<CommentResultDto>();

            foreach (var info in listUserComment)
            {
                var userInfo = await _context.NguoiDungs.Where(x => x.MaND == info.MaND).Select(x => new NguoiDungCommentDto
                {
                    AnhDaiDien = x.AnhDaiDien,
                    TenND = x.TenND
                }).FirstOrDefaultAsync();
                commentResultDtos.Add(new CommentResultDto
                {
                    tacgia = userInfo,
                    NgayBinhLuan = info.ThoiGian,
                    NoiDung = info.NoiDung
                });
            }

            return new PostAndCommentsDto
            {
                post = new PostResultDto
                {

                    TenCT = post.TenCT,
                    MoTaCT = post.MoTaCT,
                    TongCalories = post.TongCalories,
                    AnhCT = post.AnhCT,
                    LuotXem = post.LuotXem,
                    LuotLuu = post.LuotLuu,
                    LuotThich = post.LuotThich

                },

                comments = commentResultDtos


            };
        }

        public async Task<bool> LikeCongThuc(Like_Share_GetOnePostDto dto)
        {
            var account = await _account.AuthenticationAsync(new TokenModel { AccessToken = dto.token });

            if (account == null) return false;

            var post = await _context.CongThucs.FirstOrDefaultAsync(x => x.MaCT == dto.IdCongThuc);

            if (post == null) return false;

            var user = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaTK == account.Id);
            if (user == null) return false;

            var likedPost = new CTDaThich
            {
                MaCT = dto.IdCongThuc,
                MaND = user.MaND
            };

            await _context.CTDaThichs.AddAsync(likedPost);
            post.LuotThich++;
            post.LuotXem++; 

            var thongBao = new ThongBao()
            {
                MaND = post.MaND,
                NoiDung = $"{user.TenND} vừa thích bài viết của bạn",
                DaXem = false,
                NgayTao = DateTime.UtcNow,
            };
            await _context.ThongBaos.AddAsync(thongBao);

            await _context.SaveChangesAsync();
            return true;
    
        }
        public async Task<bool> CommentCongThuc(CommentPostDto comments)
        {
            var account = await _account.AuthenticationAsync(new TokenModel { AccessToken = comments.like_share.token });

            if (account == null) return false;

            var post = await _context.CongThucs.FirstOrDefaultAsync(x => x.MaCT == comments.like_share.IdCongThuc);

            if (post == null) return false;

            var user = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaTK == account.Id);
            if (user == null) return false;

            var comment = new Comment
            {
                MaND = user.MaND,
                MaBaiViet = post.MaCT,
                NoiDung = comments.Comment,
                ThoiGian = DateTime.UtcNow
            };
            await _context.Comments.AddAsync(comment);

            var thongBao = new ThongBao
            {
                MaND = post.MaND,
                NoiDung = $"{user.TenND} đã bình luận về bài viết của bạn",
                NgayTao = DateTime.UtcNow,
                DaXem = false
            };

            await _context.ThongBaos.AddAsync(thongBao);
            await _context.SaveChangesAsync(); 


            return true;
        }

        public async Task<bool> ShareCongThuc(Like_Share_GetOnePostDto dto)
        {
            var account = await _account.AuthenticationAsync(new TokenModel { AccessToken = dto.token });

            if (account == null) return false;

            var post = await _context.CongThucs.FirstOrDefaultAsync(x => x.MaCT == dto.IdCongThuc);

            if (post == null) return false;

            var user = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaTK == account.Id);
            if (user == null) return false;

          
            post.LuotThich++;
            post.LuotShare++; 

            var thongBao = new ThongBao()
            {
                MaND = post.MaND,
                NoiDung = $"{user.TenND} vừa chia sẻ bài viết của bạn",
                DaXem = false,
                NgayTao = DateTime.UtcNow,
            };
            await _context.ThongBaos.AddAsync(thongBao);

            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<CongThuc> updateCongThuc(int id, RecipeDto congthuc)
        {
            var recipe = await _context.CongThucs.FindAsync(id);
            if (recipe == null) return null!;
            _dbMapper.Map(congthuc, recipe);
            _context.Update(recipe); 
            await _context.SaveChangesAsync(); 

            return recipe;
        }
        
    }
}
