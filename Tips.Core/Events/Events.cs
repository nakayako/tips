﻿using Haskellable.Code.Monads.Maybe;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tips.Core.Services;
using Tips.Model.Models;
using Tips.Model.Models.PermissionModels;

namespace Tips.Core.Events
{
    public class AuthUserEvent : PubSubEvent<AuthOrder>
    {
    }

    public class GetUserEvent : PubSubEvent<GetOrder<IUser>>
    {
    }

    public class GetDeleteUserPermissionEvent : PubSubEvent<Action<IPermission>>
    {
    }

    public class AddUserEvent : PubSubEvent<IUser>
    {
    }

    public class DeleteUserEvent : PubSubEvent<IUser>
    {
    }

    public class AddUserIconEvent : PubSubEvent<AddUserWithIcon>
    {
    }


    public class AddProjectEvent : PubSubEvent<AddProjectOrder>
    {
    }

    public class UpdateProjectEvent : PubSubEvent<IProject>
    {
    }

    public class GetProjectEvent : PubSubEvent<GetOrder<IProject>>
    {
    }

    public class GetDeleteProjectPermissionEvent : PubSubEvent<Action<IPermission>>
    {
    }

    public class DeleteProjectEvent : PubSubEvent<IProject>
    {
    }

    public class GetTaskWithRecordEvent : PubSubEvent<GetOrder<ITaskWithRecord>>
    {
    }

    public class GetDeleteTaskRecordPermissionEvent : PubSubEvent<GetOrder<Tuple<int,int>, IPermission>>
    {
    }

    public class AddTaskCommentEvent : PubSubEvent<AddOrder<ITaskComment, int>>
    {
    }

    public class AddTaskRecordEvent : PubSubEvent<AddOrder<ITaskRecord, int>>
    {
    }

    public class DeleteTaskRecordEvent : PubSubEvent<DeleteOrder<ITaskWithRecord, int>>
    {
    }

    public class AddUserToTaskEvent : PubSubEvent<AddOrder<IUser, int>>
    {
    }

    public class GetWorkdayContextEvent : PubSubEvent<GetOrder<int, IWorkdayContext>>
    {

    }

    public class UpdateTaskEvent : PubSubEvent<ITaskItem>
    {

    }

    public static class PubSubEventExtention
    {
        public static IMaybe<TReturn> Get<TReturn>(this PubSubEvent<Action<TReturn>> @this)
        {
            var v = default(TReturn);
            var isCallback = false;
            var callback = Act.New((TReturn x) => { v = x; isCallback = true; });
            @this.Publish(callback);
            return v.ToMaybe().Where(_ => isCallback);
        }
        
        public static IMaybe<TReturn> Get<TIN, TReturn>(this PubSubEvent<GetOrder<TIN,TReturn>> @this, TIN param)
        {
            var v = default(TReturn);
            var isCallback = false;
            var order = new GetOrder<TIN, TReturn>
            {
                Param = param,
                Callback = Act.New((TReturn x) => { v = x; isCallback = true; })
            };
            @this.Publish(order);
            return v.ToMaybe().Where(_ => isCallback);
        }

        public static IUser Get(this PubSubEvent<AuthOrder> @this, IUser authUser)
        {
            var v = null as IUser;
            var callback = Act.New((IUser x) => { v = x; });
            var order = new AuthOrder
            {
                AuthUser = authUser,
                Callback = callback,
            };
            @this.Publish(order);
            return v;
        }

        public static IEnumerable<TReturn> Get<TReturn>(this PubSubEvent<GetOrder<TReturn>> @this, Func<TReturn,bool> predicate)
        {
            var v = Enumerable.Empty<TReturn>();
            var isCallback = false;
            var callback = Act.New((IEnumerable < TReturn> x) => { v = x; isCallback = true; });
            var order = new GetOrder<TReturn>
            {
                Callback = callback,
                Predicate = predicate
            };
            @this.Publish(order);

            return
                isCallback 
                ? v
                : Enumerable.Empty<TReturn>();
        }

        public static void Publish<T,With>(this PubSubEvent<AddOrder<T,With>> @this, T model, With with)
        {
            var order = new AddOrder<T, With>
            {
                Model = model,
                WithIn = with,
            };
            @this.Publish(order);
        }

        public static void Publish<T, With>(this PubSubEvent<DeleteOrder<T, With>> @this, T model, With with)
        {
            var order = new DeleteOrder<T, With>
            {
                Model = model,
                WithIn = with,
            };
            @this.Publish(order);
        }
    }
}
