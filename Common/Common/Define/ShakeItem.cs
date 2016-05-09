using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Common.Define
{
    [Serializable]
    public class ShakeItem
    {
        [Key]
        public string Id { get; set; }
        [Display(Name = "姓名")]
        public string Name { get; set; }
        [Display(Name = "手机")]
        //[RegularExpression(@"^[1][358][0-9]{9}$", ErrorMessage = "错了")]
        public string Telephone { get; set; }
        [Display(Name = "地域")]
        public string Area { get; set; }
        [Display(Name = "成绩")]
        //[Range(0, 100, ErrorMessage = "成绩不能大于{2} 不能小于{1}")]
        public decimal Score { get; set; }

        public ShakeItem()
        {
            Id = Name = Telephone = Area = String.Empty;
            Score = 0;
        }
    }

    public class ShakeItemDbContext : GenericDbContext<ShakeItem>
    {

    }

    public class GenericDbContext<T> : DbContext where T : class
    {
        public DbSet<T> Items { get; set; }
        public DbSet<T> Get()
        {
            return Set<T>();
        }

        public T Get(params object[] keyValues)
        {
            return Items.Find(keyValues);
        }

        public void Modify(T item)
        {
            Items.Attach(item);
            Entry(item).State = EntityState.Modified;
            SaveChanges();
        }

        public void Add(T item)
        {
            Items.Add(item);
            SaveChanges();
        }

        public void Delete(params object[] keyValues)
        {
            Delete(Get(keyValues));
        }

        public void Delete(T item)
        {
            Items.Attach(item);
            Entry(item).State = EntityState.Deleted;
            SaveChanges();
        }
    }
}
