using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using DashBoard.Business.DTOs.Domains;
using DashBoard.Business.Services;
using DashBoard.Data.Data;
using DashBoard.Data.Entities;
using DashBoard.Data.Enums;
using DashBoard.Web.Helpers;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Xunit;

namespace Services.Tests
{

    //I recommend checking helper class at the bottom and get familiar with the seed data we use to populate databases in tests. When you know what data we have, these tests are self explanatory.
    public class TestDomainService
    {
        [Fact]
        public void TestGetById()
        {   
            //Arrange
            var options = new DbContextOptionsBuilder<DataContext>() //instead of mocking we use inMemoryDatabase.
                .UseInMemoryDatabase(databaseName: "TestGetById")
                .Options;
            
            var config = new MapperConfiguration(cfg =>
                cfg.AddProfile<AutoMapperProfile>());

            var mapper = config.CreateMapper(); // according to some people this is better than mocking automapper.
            //Act
            using (var context = new DataContext(options))
            {
                context.Domains.AddRange(SeedFakeData.SeedEntitiesDomainModels());
                context.Users.AddRange(SeedFakeData.SeedEntitiesUsersModels());
                context.SaveChanges();
            }
            
            using (var context = new DataContext(options))
            {
                var service = new DomainService(context, mapper);
                var okResult = service.GetById(1, "1");
                var badResult = service.GetById(10000, "1"); 
            //Assert
                Assert.Null(badResult);
                Assert.IsType<DomainModelDto>(okResult);
                Assert.Equal(1, okResult.Id);
            }
        }
        [Fact]
        public void TestGetAllNotDeleted()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<DataContext>() //instead of mocking we use inMemoryDatabase.
                .UseInMemoryDatabase(databaseName: "TestGetAllNotDeleted")
                .Options;

            var config = new MapperConfiguration(cfg =>
                cfg.AddProfile<AutoMapperProfile>());

            var mapper = config.CreateMapper(); // according to some people this is better than mocking automapper.
            //Act
            using (var context = new DataContext(options))
            {
                context.Domains.AddRange(SeedFakeData.SeedEntitiesDomainModels());
                context.Users.AddRange(SeedFakeData.SeedEntitiesUsersModels());

                context.SaveChanges();
            }
            
            using (var context = new DataContext(options))
            {
                var service = new DomainService(context, mapper);
                var result = service.GetAllNotDeleted("1"); //user that has id 1. This comes from controller in production.
                //Assert
                Assert.Equal(2, result.Count());
                Assert.IsType<List<DomainModelDto>>(result);
                foreach (var domainModel in result)
                {
                   Assert.False(domainModel.Deleted); 
                }
            }
        }
        [Fact]
        public async void TestCreate()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<DataContext>() //instead of mocking we use inMemoryDatabase.
                .UseInMemoryDatabase(databaseName: "TestCreate")
                .Options;

            var config = new MapperConfiguration(cfg =>
                cfg.AddProfile<AutoMapperProfile>());

            var mapper = config.CreateMapper(); // according to some people this is better than mocking automapper.

            var testModel = new DomainForCreationDto()
            {
                Service_Name = "service1",
                Url = "www.google.com",
                Notification_Email = "admin1@admin.com"
            };
            //Act
            using (var context = new DataContext(options))
            {
                context.Domains.AddRange(SeedFakeData.SeedEntitiesDomainModels());
                context.Users.AddRange(SeedFakeData.SeedEntitiesUsersModels());

                context.SaveChanges();
            }
            using (var context = new DataContext(options))
            {
                var service = new DomainService(context, mapper);
                var result = await service.Create(testModel, "1");
                Assert.Equal(default, result.Parameters); //check for some un-set property if it becomes default after Create service
                Assert.False(result.Deleted); 
                Assert.IsType<DomainModelDto>(result);
                Assert.Equal(5, context.Domains.Count());
            }
        }

        [Fact]
        public void TestUpdate()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<DataContext>() //instead of mocking we use inMemoryDatabase.
                .UseInMemoryDatabase(databaseName: "TestUpdate")
                .Options;

            var config = new MapperConfiguration(cfg =>
                cfg.AddProfile<AutoMapperProfile>());

            var mapper = config.CreateMapper(); // according to some people this is better than mocking automapper.
            using (var context = new DataContext(options))
            {
                context.Domains.AddRange(SeedFakeData.SeedEntitiesDomainModels());
                context.Users.AddRange(SeedFakeData.SeedEntitiesUsersModels());
                context.SaveChanges();
            }

            var testModel = new DomainForUpdateDto()
            {
                Service_Name = "newServiceName", Url = "http://www.festo.com", Service_Type = ServiceType.ServiceRest,
                Method = RequestMethod.Get, Basic_Auth = true, Auth_Password = null, Auth_User = null,
                Notification_Email = "aadas1@aaa.com", Interval_Ms = 30000, Parameters = null, Active = true
            };
            //Act
            using (var context = new DataContext(options))
            {
                var service = new DomainService(context, mapper);
                var result = service.Update(1, testModel, "1"); 

                //Assert
                Assert.Equal(4,context.Domains.Count());
                Assert.NotNull(result);
                Assert.Equal("newServiceName", context.Domains.Find(1).Service_Name); //check if property was updated
            }
        }

        [Fact]
        public void TestPseudoDelete()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<DataContext>() //instead of mocking we use inMemoryDatabase.
                .UseInMemoryDatabase(databaseName: "TestPseudoDelete")
                .Options;

            var config = new MapperConfiguration(cfg =>
                cfg.AddProfile<AutoMapperProfile>());

            var mapper = config.CreateMapper(); // according to some people this is better than mocking automapper.
            using (var context = new DataContext(options))
            {
                context.Domains.AddRange(SeedFakeData.SeedEntitiesDomainModels());
                context.Users.AddRange(SeedFakeData.SeedEntitiesUsersModels());
                context.SaveChanges();
            }

            //Act
            using (var context = new DataContext(options))
            {
                var service = new DomainService(context, mapper);
                var resultDeleted = service.PseudoDelete(2, "1"); //same teamKey
                var resultWrongTeam = service.PseudoDelete(4, "1"); //different teamKey
                //Assert
                Assert.Equal(4, context.Domains.Count()); //same number as before. It's pseudo delete for a reason.
                Assert.True(context.Domains.Find(2).Deleted);
                Assert.False(context.Domains.Find(4).Deleted);
            }
        }

        internal static class SeedFakeData
        {
            private static readonly Guid FirstTeamKey = Guid.NewGuid();
            private static readonly Guid SecondTeamKey = Guid.NewGuid();

            //creates fake users for database
            internal static IEnumerable<User> SeedEntitiesUsersModels()
            {
                var list = new List<User>
                {
                    new User() { Id = 1, Team_Key = FirstTeamKey },
                    new User() { Id = 2, Team_Key = FirstTeamKey },
                    new User() { Id = 3, Team_Key = SecondTeamKey }
                };
                return list;
            }
            //creates fake domains for database
            internal static IEnumerable<DomainModel> SeedEntitiesDomainModels()
            {
                var list = new List<DomainModel>
                {
                    new DomainModel { Id = 1, Service_Name = "service1", Url = "http://www.festo.com", Service_Type = ServiceType.ServiceRest, Method = RequestMethod.Get, Basic_Auth = false, Auth_Password = null, Auth_User = null, Notification_Email = "aadas1@aaa.com", Interval_Ms = 30000, Parameters = null, Active = true, Deleted = false, Created_By = 0, Modified_By = 0, Date_Created = DateTime.Now, Date_Modified = DateTime.Now, Last_Fail = DateTime.Now, Team_Key = FirstTeamKey },
                    new DomainModel { Id = 2, Service_Name = "service2", Url = "http://www.google.com", Service_Type = ServiceType.ServiceSoap, Method = RequestMethod.Get, Basic_Auth = false, Auth_Password = null, Auth_User = null, Notification_Email = "aadas2@aaa.com", Interval_Ms = 60000, Parameters = "random parameters", Active = true, Deleted = false, Created_By = 0, Modified_By = 0, Date_Created = DateTime.Now, Date_Modified = DateTime.Now, Last_Fail = DateTime.Now, Team_Key = FirstTeamKey },
                    new DomainModel { Id = 3, Service_Name = "service3", Url = "http://www.kitm.lt", Service_Type = ServiceType.ServiceRest, Method = RequestMethod.Get, Basic_Auth = true, Auth_Password = "password123", Auth_User = "username", Notification_Email = "aadas3@aaa.com", Interval_Ms = 70000, Parameters = "random parameters", Active = false, Deleted = true, Created_By = 0, Modified_By = 0, Date_Created = DateTime.Now, Date_Modified = DateTime.Now, Last_Fail = DateTime.Now, Team_Key = SecondTeamKey },
                    new DomainModel { Id = 4, Service_Name = "service4", Url = "http://www.facebook.com", Service_Type = ServiceType.ServiceRest, Method = RequestMethod.Post, Basic_Auth = false, Auth_Password = null, Auth_User = null, Notification_Email = "aada4@aaa.com", Interval_Ms = 55000, Parameters = "random parameters", Active = true, Deleted = false, Created_By = 0, Modified_By = 0, Date_Created = DateTime.Now, Date_Modified = DateTime.Now, Last_Fail = DateTime.Now, Team_Key = SecondTeamKey }
                };
                return list;
            }
        }
    }
}
