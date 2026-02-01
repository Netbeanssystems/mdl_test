using System;
namespace Application.Helpers
{
    public static class ModelAuditor<T> where T : class
    {
        public static T SetAudit(string username, string action, string ip, T model)
        {
            var tempModel = typeof(T);
            //foreach (var pr in model.GetType().GetProperties())
            //{
            //    if (pr.PropertyType == typeof(string))
            //        pr.SetValue(model, pr.GetValue(model)?.ToString()?.Trim());
            //}
            if (action == "Create")
            {
                tempModel.GetProperty("CreatedDate")?.SetValue(model, DateTime.UtcNow.AddHours(5.5));
                tempModel.GetProperty("CreatedBy")?.SetValue(model, username);
                //tempModel.GetProperty("IsActive")?.SetValue(model, true);
            }
            if (action == "Edit")
            {
                tempModel.GetProperty("CreatedDate")?.SetValue(model, DateTime.UtcNow.AddHours(5.5));
                tempModel.GetProperty("ModifiedDate")?.SetValue(model, DateTime.UtcNow.AddHours(5.5));
                tempModel.GetProperty("ModifiedBy")?.SetValue(model, username);
            }
            tempModel.GetProperty("IP")?.SetValue(model, ip);
            tempModel.GetProperty("IsActive")?.SetValue(model, true);
            return model;
        }
        //public static T TrimModel(T model)
        //{
        //    foreach (var pr in model.GetType().GetProperties())
        //    {
        //        if (pr.PropertyType == typeof(string))
        //            pr.SetValue(model, pr.GetValue(model)?.ToString()?.Trim());
        //    }
        //    return model;
        //}
    }
}
