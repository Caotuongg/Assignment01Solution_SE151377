using Repositories.Entities;
using Repositories.Repositories.Interface;
using Repositories.Ultilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IMemberService
    {
        Task<Member> Get(int id);
        IEnumerable<Member> GetAll();
        Task<bool> Add(Member member);
        Task<bool> Update(Member member);
        Task<bool> Delete(int id);
        Task<Member> Login(string email, string password);

    }
    public class MemberService : IMemberService
    {
        private readonly IAllUnit allUnit;
        public MemberService(IAllUnit allUnit) 
        {
            this.allUnit = allUnit;
        }
        //private readonly IMemberRepository memberRepository;
        //public MemberService(IMemberRepository memberRepository)
        //{
        //    this.memberRepository = memberRepository;
        //}

        public Task<bool> Add(Member member)
        {
            try
            {
                //if (memberRepository.GetAll().Any())
                //{
                //    throw new Exception("Email exist");
                //}
                //member.IngredientId = AutoGenId.AutoGenerateId();
                //ingredient.IsDelete = true;
                return allUnit.MemberRepository.Add(member);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<Member> Get(int id)
        {
            try
            {
                return allUnit.MemberRepository.Get(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Member> GetAll()
        {
            try
            {
                return allUnit.MemberRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<bool> Delete(int id)
        {
            try
            {
                var member = allUnit.MemberRepository.Get(id);
                if (member != null)
                {

                    return  allUnit.MemberRepository.Delete(id);
                }
                else
                {
                    throw new Exception("Not Found Member");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Update(Member item)
        {
            try
            {
                var member = await allUnit.MemberRepository.Get(item.MemberId);
                //if (memberRepository.GetAll(x => x.MemberId != item.MemberId && x.Email == item.Email).Any())
                //{
                //    throw new Exception("Email exist");
                //}
                member.CompanyName = item.CompanyName;
                member.City = item.City;
                member.Country = item.Country;


                return await allUnit.MemberRepository.Update(member.MemberId, member);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Member> Login(string email, string password)
        {
            
            try
            {
                var account = await allUnit.MemberRepository.GetAllMember();
                return account.SingleOrDefault(x => x.Email == email && x.Password == password);
                
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
    }

