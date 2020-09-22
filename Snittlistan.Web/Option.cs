﻿namespace Snittlistan.Web
{
    using System;

    public abstract class Option<T>
    {
        public static implicit operator Option<T>(T value) =>
            new Some<T>(value);

        public static implicit operator Option<T>(None none) =>
            new None<T>();

        public abstract Option<TResult> Map<TResult>(Func<T, TResult> map);
        public abstract Option<TResult> MapOptional<TResult>(Func<T, Option<TResult>> map);
        public abstract T Reduce(T whenNone);
        public abstract T Reduce(Func<T> whenNone);
        public abstract void Match(Action<T> whenSome, Action whenNone);
        public abstract U Match<U>(Func<T, U> whenSome, Func<U> whenNone);
    }

    public sealed class Some<T> : Option<T>
    {
        public T Content { get; }

        public Some(T value)
        {
            Content = value;
        }

        public static implicit operator T(Some<T> some) =>
            some.Content;

        public override Option<TResult> Map<TResult>(Func<T, TResult> map) =>
            map(Content);

        public override Option<TResult> MapOptional<TResult>(Func<T, Option<TResult>> map) =>
            map(Content);

        public override T Reduce(T whenNone) =>
            Content;

        public override T Reduce(Func<T> whenNone) =>
            Content;

        public override void Match(Action<T> whenSome, Action whenNone)
        {
            whenSome.Invoke(Content);
        }

        public override U Match<U>(Func<T, U> whenSome, Func<U> whenNone)
        {
            return whenSome.Invoke(Content);
        }
    }

    public sealed class None<T> : Option<T>
    {
        public override Option<TResult> Map<TResult>(Func<T, TResult> map) =>
            None.Value;

        public override Option<TResult> MapOptional<TResult>(Func<T, Option<TResult>> map) =>
            None.Value;

        public override void Match(Action<T> whenSome, Action whenNone)
        {
            whenNone.Invoke();
        }

        public override U Match<U>(Func<T, U> whenSome, Func<U> whenNone)
        {
            return whenNone.Invoke();
        }

        public override T Reduce(T whenNone) =>
            whenNone;

        public override T Reduce(Func<T> whenNone) =>
            whenNone();
    }

    public sealed class None
    {
        public static None Value { get; } = new None();

        private None() { }
    }
}
