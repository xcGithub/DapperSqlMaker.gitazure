﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Linq.Expressions;
using Dapper;
using Dapper.Contrib.Extensions;


namespace DapperSqlMaker.DapperExt
{

    /// <summary>
    /// 联表类型
    /// </summary>
    public enum JoinType
    {
        Left
        , Right
        , Inner
    }


    /// <summary>
    /// 6表查询
    /// </summary>
    /// <typeparam name="T">主表</typeparam>
    /// <typeparam name="Y">联表2</typeparam>
    /// <typeparam name="Z">联表3</typeparam>
    /// <typeparam name="O">联表4</typeparam>
    /// <typeparam name="P">联表5</typeparam>
    /// <typeparam name="Q">联表6</typeparam>
    public abstract class DapperSqlMaker<T, Y, Z, O, P, Q> : DapperSqlMaker
    {
        public abstract override IDbConnection GetConn();
        public DapperSqlMaker<T, Y, Z, O, P, Q> Select()
        {
            //1.存表序号和表别名 //
            var tabAliasName1 = "a"; //fielambda.Parameters[0].Name;
            var tabAliasName2 = "b"; //fielambda.Parameters[1].Name;
            var tabAliasName3 = "c"; //fielambda.Parameters[2].Name;
            var tabAliasName4 = "d"; //fielambda.Parameters[3].Name;


            var tabAliasName6 = "f"; //fielambda.Parameters[3].Name;
            var tab6fname = typeof(Q).FullName;
            if (!TabAliaceDic.ContainsKey(tab6fname)) TabAliaceDic.Add(tab6fname + 6, tabAliasName6);

            var tabAliasName5 = "e"; //fielambda.Parameters[3].Name;
            var tab5fname = typeof(P).FullName;
            if (!TabAliaceDic.ContainsKey(tab5fname)) TabAliaceDic.Add(tab5fname + 5, tabAliasName5);

            TabAliaceDic.Add(typeof(T).FullName + 1, tabAliasName1);
            var tab2fname = typeof(Y).FullName;
            if (!TabAliaceDic.ContainsKey(tab2fname)) TabAliaceDic.Add(tab2fname + 2, tabAliasName2);
            var tab3fname = typeof(Z).FullName;
            if (!TabAliaceDic.ContainsKey(tab3fname)) TabAliaceDic.Add(tab3fname + 3, tabAliasName3);
            var tab4fname = typeof(O).FullName;
            if (!TabAliaceDic.ContainsKey(tab4fname)) TabAliaceDic.Add(tab4fname + 4, tabAliasName4);

            Clauses.Add(Clause.New(ClauseType.ActionSelect, select: " select "));
            return this;
        }
        /// <summary>
        /// MSSQL RowRumber OrderBy 字段  p => p.Id  orderfiesExps必须有返回值 后面的orderby就不要拼接了
        /// </summary> 
        public DapperSqlMaker<T, Y, Z, O, P, Q> RowRumberOrderBy(Expression<Func<T, Y, Z, O, P, Q, object>> orderfiesExps)
        {
            LambdaExpression fielambda = orderfiesExps as LambdaExpression;
            if (!(fielambda.Body is NewExpression)) throw new Exception("RowRumber字段未解析");
            // 2. 解析RowRumber_OrderBy字段
            string columns;

            // 查指定字段 
            NewExpression arryexps = fielambda.Body as NewExpression;
            Dictionary<string, int> pdic = base.GetLmdparamsDic(fielambda);
            columns = GetFieldrrExps(arryexps.Arguments, pdic); //"select " +
            columns = string.Format(SM.LimitRowNumber_Sql, columns);

            Clauses.Add(Clause.New(ClauseType.ActionSelectRowRumberOrderBy, rowRumberOrderBy: columns));
            return this;
        }
        /// <summary>
        /// 查询指定字段(默认查询*所有字段) 匿名类型传入Fileds t =>  new { t.f1, t.f2, t2.f3 }   ==>   tab1.f1, tab1.f2, tab2.f3
        /// </summary> 
        public DapperSqlMaker<T, Y, Z, O, P, Q> Column(Expression<Func<T, Y, Z, O, P, Q, object>> fiesExps = null)
        {
            string columns;
            if (fiesExps == null) { columns = SM.ColumnAll; goto columnsall; }

            LambdaExpression fielambda = fiesExps as LambdaExpression;
            columns = base.GetColumnStr(fielambda);

            columnsall:
            Clauses.Add(Clause.New(ClauseType.ActionSelectColumn, selectColumn: columns));
            return this;
        }
        public DapperSqlMaker<T, Y, Z, O, P, Q> FromJoin(
            JoinType joinType2, Expression<Func<T, Y, Z, O, P, Q, bool>> joinExps2
          , JoinType joinType3, Expression<Func<T, Y, Z, O, P, Q, bool>> joinExps3
          , JoinType joinType4, Expression<Func<T, Y, Z, O, P, Q, bool>> joinExps4
          , JoinType joinType5, Expression<Func<T, Y, Z, O, P, Q, bool>> joinExps5
          , JoinType joinType6, Expression<Func<T, Y, Z, O, P, Q, bool>> joinExps6)
        {
            // 1. 表别名
            var tabAliasName1 = "a";
            var tabAliasName2 = "b";
            var tabAliasName3 = "c";
            var tabAliasName4 = "d";
            var tabAliasName5 = "e";
            var tabAliasName6 = "f";

            // 2. 主表和查询字段 sel ... from tab
            var tabname1 = DsmSqlMapperExtensions.GetTableName(typeof(T));
            var selstr = $" from {tabname1} {tabAliasName1}"; // sel .. from 表明 别名
            // 3. 生成联表2sql join tab on ...
            string jointabstr2 = GetJoinTabStr(typeof(Y), tabAliasName2, joinType2, joinExps2);
            // 生成联表3sql
            string jointabstr3 = GetJoinTabStr(typeof(Z), tabAliasName3, joinType3, joinExps3);
            // 生成联表4sql
            string jointabstr4 = GetJoinTabStr(typeof(O), tabAliasName4, joinType4, joinExps4);
            // 生成联表5sql
            string jointabstr5 = GetJoinTabStr(typeof(P), tabAliasName5, joinType5, joinExps5);
            // 生成联表6sql
            string jointabstr6 = GetJoinTabStr(typeof(Q), tabAliasName6, joinType6, joinExps6);
            Clauses.Add(Clause.New(ClauseType.ActionSelectFrom, fromJoin: selstr + jointabstr2 + jointabstr3 + jointabstr4 + jointabstr5 + jointabstr6));
            return this;
        }
        public DapperSqlMaker<T, Y, Z, O, P, Q> Where(Expression<Func<T, Y, Z, O, P, Q, bool>> whereExps)
        {
            DynamicParameters spars;
            string sqlCondition;
            base.GetWhereStr(whereExps, out spars, out sqlCondition);

            Clauses.Add(Clause.New(ClauseType.ActionSelectWhereOnHaving, condition: sqlCondition, conditionParms: spars));

            return this;
        }
        /// <param name="fiesExps"></param>
        /// <param name="isDesc">最后一个字段是否降序</param>
        /// <returns></returns>
        public DapperSqlMaker<T, Y, Z, O, P, Q> Order(Expression<Func<T, Y, Z, O, P, Q, object>> fiesExps, bool isDesc = false)
        {
            LambdaExpression fielambda = fiesExps as LambdaExpression;
            string columns = base.GetOrderStr(fielambda, isDesc);
            Clauses.Add(Clause.New(ClauseType.ActionSelectOrder, order: columns));
            return this;
        }

    }

    /// <summary>
    /// 5表查询
    /// </summary>
    /// <typeparam name="T">主表</typeparam>
    /// <typeparam name="Y">联表2</typeparam>
    /// <typeparam name="Z">联表3</typeparam>
    /// <typeparam name="O">联表4</typeparam>
    /// <typeparam name="P">联表5</typeparam>
    public abstract class DapperSqlMaker<T, Y, Z, O, P> : DapperSqlMaker
    {
        public abstract override IDbConnection GetConn();
        public DapperSqlMaker<T, Y, Z, O, P> Select()
        {
            //1.存表序号和表别名 //FromJoin 得用到
            var tabAliasName1 = "a"; //fielambda.Parameters[0].Name;
            var tabAliasName2 = "b"; //fielambda.Parameters[1].Name;
            var tabAliasName3 = "c"; //fielambda.Parameters[2].Name;
            var tabAliasName4 = "d"; //fielambda.Parameters[3].Name;

            var tabAliasName5 = "e"; //fielambda.Parameters[3].Name;
            var tab5fname = typeof(P).FullName;
            if (!TabAliaceDic.ContainsKey(tab5fname)) TabAliaceDic.Add(tab5fname + 5, tabAliasName5);

            TabAliaceDic.Add(typeof(T).FullName + 1, tabAliasName1);
            var tab2fname = typeof(Y).FullName;
            if (!TabAliaceDic.ContainsKey(tab2fname)) TabAliaceDic.Add(tab2fname + 2, tabAliasName2);
            var tab3fname = typeof(Z).FullName;
            if (!TabAliaceDic.ContainsKey(tab3fname)) TabAliaceDic.Add(tab3fname + 3, tabAliasName3);
            var tab4fname = typeof(O).FullName;
            if (!TabAliaceDic.ContainsKey(tab4fname)) TabAliaceDic.Add(tab4fname + 4, tabAliasName4);

            Clauses.Add(Clause.New(ClauseType.ActionSelect, select: " select "));
            return this;
        }
        /// <summary>
        /// MSSQL RowRumber OrderBy 字段  p => p.Id  orderfiesExps必须有返回值 后面的orderby就不要拼接了
        /// </summary> 
        public DapperSqlMaker<T, Y, Z, O, P> RowRumberOrderBy(Expression<Func<T, Y, Z, O, P, object>> orderfiesExps)
        {
            LambdaExpression fielambda = orderfiesExps as LambdaExpression;
            if (!(fielambda.Body is NewExpression)) throw new Exception("RowRumber字段未解析");
            // 2. 解析RowRumber_OrderBy字段
            string columns;

            // 查指定字段 
            NewExpression arryexps = fielambda.Body as NewExpression;
            Dictionary<string, int> pdic = base.GetLmdparamsDic(fielambda);
            columns = GetFieldrrExps(arryexps.Arguments, pdic); //"select " +
            columns = string.Format(SM.LimitRowNumber_Sql, columns);

            Clauses.Add(Clause.New(ClauseType.ActionSelectRowRumberOrderBy, rowRumberOrderBy: columns));
            return this;
        }
        /// <summary>
        /// 查询指定字段(默认查询*所有字段) 匿名类型传入Fileds t =>  new { t.f1, t.f2, t2.f3 }   ==>   tab1.f1, tab1.f2, tab2.f3
        /// </summary> 
        public DapperSqlMaker<T, Y, Z, O, P> Column(Expression<Func<T, Y, Z, O, P, object>> fiesExps = null)
        {
            string columns;
            if (fiesExps == null) { columns = SM.ColumnAll; goto columnsall; }

            LambdaExpression fielambda = fiesExps as LambdaExpression;
            columns = base.GetColumnStr(fielambda);

            columnsall:
            Clauses.Add(Clause.New(ClauseType.ActionSelectColumn, selectColumn: columns));
            return this;
        }
        public DapperSqlMaker<T, Y, Z, O, P> FromJoin(
            JoinType joinType2, Expression<Func<T, Y, Z, O, P, bool>> joinExps2
          , JoinType joinType3, Expression<Func<T, Y, Z, O, P, bool>> joinExps3
          , JoinType joinType4, Expression<Func<T, Y, Z, O, P, bool>> joinExps4
          , JoinType joinType5, Expression<Func<T, Y, Z, O, P, bool>> joinExps5)
        {
            // 1. 表别名
            var tabAliasName1 = "a";
            var tabAliasName2 = "b";
            var tabAliasName3 = "c";
            var tabAliasName4 = "d";
            var tabAliasName5 = "e";

            // 2. 主表和查询字段 sel ... from tab
            var tabname1 = DsmSqlMapperExtensions.GetTableName(typeof(T));
            var selstr = $" from {tabname1} {tabAliasName1}"; // sel .. from 表明 别名
            // 3. 生成联表2sql join tab on ...
            string jointabstr2 = GetJoinTabStr(typeof(Y), tabAliasName2, joinType2, joinExps2);
            // 生成联表3sql
            string jointabstr3 = GetJoinTabStr(typeof(Z), tabAliasName3, joinType3, joinExps3);
            // 生成联表4sql
            string jointabstr4 = GetJoinTabStr(typeof(O), tabAliasName4, joinType4, joinExps4);
            // 生成联表5sql
            string jointabstr5 = GetJoinTabStr(typeof(P), tabAliasName5, joinType5, joinExps5);
            Clauses.Add(Clause.New(ClauseType.ActionSelectFrom, fromJoin: selstr + jointabstr2 + jointabstr3 + jointabstr4 + jointabstr5));
            return this;
        }
        public DapperSqlMaker<T, Y, Z, O, P> Where(Expression<Func<T, Y, Z, O, P, bool>> whereExps)
        {
            DynamicParameters spars;
            string sqlCondition;
            base.GetWhereStr(whereExps, out spars, out sqlCondition);

            Clauses.Add(Clause.New(ClauseType.ActionSelectWhereOnHaving, condition: sqlCondition, conditionParms: spars));

            return this;
        }
        /// <param name="fiesExps"></param>
        /// <param name="isDesc">最后一个字段是否降序</param>
        /// <returns></returns>
        public DapperSqlMaker<T, Y, Z, O, P> Order(Expression<Func<T, Y, Z, O, P, object>> fiesExps, bool isDesc = false)
        {
            LambdaExpression fielambda = fiesExps as LambdaExpression;
            string columns = base.GetOrderStr(fielambda, isDesc);
            Clauses.Add(Clause.New(ClauseType.ActionSelectOrder, order: columns));
            return this;
        }


    }

    /// <summary>
    /// 4表查询
    /// </summary>
    /// <typeparam name="T">主表</typeparam>
    /// <typeparam name="Y">联表2</typeparam>
    /// <typeparam name="Z">联表3</typeparam>
    /// <typeparam name="O">联表4</typeparam>
    public abstract class DapperSqlMaker<T, Y, Z, O> : DapperSqlMaker
    {
        public abstract override IDbConnection GetConn();

        public DapperSqlMaker<T, Y, Z, O> Select()
        {
            // 1. 存表序号和表别名 //FromJoin 得用到
            var tabAliasName1 = "a"; //fielambda.Parameters[0].Name;
            var tabAliasName2 = "b"; //fielambda.Parameters[1].Name;
            var tabAliasName3 = "c"; //fielambda.Parameters[2].Name;
            var tabAliasName4 = "d"; //fielambda.Parameters[3].Name;
            TabAliaceDic.Add(typeof(T).FullName + 1, tabAliasName1);
            var tab2fname = typeof(Y).FullName;
            if (!TabAliaceDic.ContainsKey(tab2fname)) TabAliaceDic.Add(tab2fname + 2, tabAliasName2);
            var tab3fname = typeof(Z).FullName;
            if (!TabAliaceDic.ContainsKey(tab3fname)) TabAliaceDic.Add(tab3fname + 3, tabAliasName3);
            var tab4fname = typeof(O).FullName;
            if (!TabAliaceDic.ContainsKey(tab4fname)) TabAliaceDic.Add(tab4fname + 4, tabAliasName4);

            Clauses.Add(Clause.New(ClauseType.ActionSelect, select: " select "));
            return this;
        }
        /// <summary>
        /// MSSQL RowRumber OrderBy 字段  p => p.Id  orderfiesExps必须有返回值 后面的orderby就不要拼接了
        /// </summary> 
        public DapperSqlMaker<T, Y, Z, O> RowRumberOrderBy(Expression<Func<T, Y, Z, O, object>> orderfiesExps)
        {

            LambdaExpression fielambda = orderfiesExps as LambdaExpression;
            if (!(fielambda.Body is NewExpression)) throw new Exception("RowRumber字段未解析");
            // 2. 解析RowRumber_OrderBy字段
            string columns;

            // 查指定字段 
            NewExpression arryexps = fielambda.Body as NewExpression;
            Dictionary<string, int> pdic = base.GetLmdparamsDic(fielambda);
            columns = GetFieldrrExps(arryexps.Arguments, pdic); //"select " +  //TabAliaceDic
            columns = string.Format(SM.LimitRowNumber_Sql, columns);
            /*
             * else if (p.NodeType == ExpressionType.Call)
            //{ 
            //    MethodCallExpression method = p as MethodCallExpression;
            //    if (method.Method.Name == SM.LimitRowNumber_Name) goto LimitRowNumber;

            //    LimitRowNumber:
            //    MemberExpression Member = method.Arguments[0] as MemberExpression;
            //    ParameterExpression Parmexr = Member.Expression as ParameterExpression;
            //    var rntkey = Parmexr.Type.FullName + paramsdic[Parmexr.Name];
            //    var mberName = tabalis[rntkey] + "." + Member.Member.Name;  //Parmexr.Name 表.字段名 
            //    var rnname = string.Format(SM.LimitRowNumber_Sql, mberName);
            //    return rnname;
            } 
            */
            Clauses.Add(Clause.New(ClauseType.ActionSelectRowRumberOrderBy, rowRumberOrderBy: columns));
            return this;
        }
        /// <summary>
        /// 查询指定字段(默认查询*所有字段) 匿名类型传入Fileds t =>  new { t.f1, t.f2, t2.f3 }   ==>   tab1.f1, tab1.f2, tab2.f3
        /// </summary> 
        public DapperSqlMaker<T, Y, Z, O> Column(Expression<Func<T, Y, Z, O, object>> fiesExps = null)
        {
            string columns;
            if (fiesExps == null) { columns = SM.ColumnAll; goto columnsall; }

            LambdaExpression fielambda = fiesExps as LambdaExpression;
            columns = base.GetColumnStr(fielambda);

            columnsall:
            Clauses.Add(Clause.New(ClauseType.ActionSelectColumn, selectColumn: columns));
            return this;
        }
        public DapperSqlMaker<T, Y, Z, O> FromJoin(
            JoinType joinType2, Expression<Func<T, Y, Z, O, bool>> joinExps2
          , JoinType joinType3, Expression<Func<T, Y, Z, O, bool>> joinExps3
          , JoinType joinType4, Expression<Func<T, Y, Z, O, bool>> joinExps4)
        {
            var tabAliasName1 = "a";
            var tabAliasName2 = "b";
            var tabAliasName3 = "c";
            var tabAliasName4 = "d";
            // 3. 主表和查询字段 sel ... from tab
            var tabname1 = DsmSqlMapperExtensions.GetTableName(typeof(T));
            var selstr = $" from {tabname1} {tabAliasName1}"; // sel .. from 表明 别名
            // 4. 生成联表2sql join tab on ...
            string jointabstr2 = GetJoinTabStr(typeof(Y), tabAliasName2, joinType2, joinExps2);
            // 5. 生成联表3sql
            string jointabstr3 = GetJoinTabStr(typeof(Z), tabAliasName3, joinType3, joinExps3);
            // 6. 生成联表4sql
            string jointabstr4 = GetJoinTabStr(typeof(O), tabAliasName4, joinType4, joinExps4);
            Clauses.Add(Clause.New(ClauseType.ActionSelectFrom, fromJoin: selstr + jointabstr2 + jointabstr3 + jointabstr4));
            return this;
        }

        public DapperSqlMaker<T, Y, Z, O> Where(Expression<Func<T, Y, Z, O, bool>> whereExps)
        {
            DynamicParameters spars;
            string sqlCondition;
            base.GetWhereStr(whereExps, out spars, out sqlCondition);

            Clauses.Add(Clause.New(ClauseType.ActionSelectWhereOnHaving, condition: sqlCondition, conditionParms: spars));

            return this;
        }
        /// <param name="fiesExps"></param>
        /// <param name="isDesc">是否降序</param>
        /// <returns></returns>
        public DapperSqlMaker<T, Y, Z, O> Order(Expression<Func<T, Y, Z, O, object>> fiesExps, bool isDesc = false)
        {
            LambdaExpression fielambda = fiesExps as LambdaExpression;
            string columns = base.GetOrderStr(fielambda, isDesc);
            Clauses.Add(Clause.New(ClauseType.ActionSelectOrder, order: columns));
            return this;
        }

    }

    /// <summary>
    /// 3表查询
    /// </summary>
    /// <typeparam name="T">主表</typeparam>
    /// <typeparam name="Y">联表1</typeparam>
    /// <typeparam name="Z">联表2</typeparam>
    public abstract class DapperSqlMaker<T, Y, Z> : DapperSqlMaker
    {
        public abstract override IDbConnection GetConn();

        public DapperSqlMaker<T, Y, Z> Select()
        {
            // 1. 存表序号和表别名 //FromJoin 得用到
            var tabAliasName1 = "a"; //fielambda.Parameters[0].Name;
            var tabAliasName2 = "b"; //fielambda.Parameters[1].Name;
            var tabAliasName3 = "c"; //fielambda.Parameters[2].Name;
            TabAliaceDic.Add(typeof(T).FullName + 1, tabAliasName1);
            var tab2fname = typeof(Y).FullName;
            if (!TabAliaceDic.ContainsKey(tab2fname)) TabAliaceDic.Add(tab2fname + 2, tabAliasName2);
            var tab3fname = typeof(Z).FullName;
            if (!TabAliaceDic.ContainsKey(tab3fname)) TabAliaceDic.Add(tab3fname + 3, tabAliasName3);

            Clauses.Add(Clause.New(ClauseType.ActionSelect, select: " select "));
            return this;
        }
        /// <summary>
        /// MSSQL RowRumber OrderBy 字段  p => p.Id  orderfiesExps必须有返回值 后面的orderby就不要拼接了
        /// </summary> 
        public DapperSqlMaker<T, Y, Z> RowRumberOrderBy(Expression<Func<T, Y, Z, object>> orderfiesExps)
        {
            LambdaExpression fielambda = orderfiesExps as LambdaExpression;
            if (!(fielambda.Body is NewExpression)) throw new Exception("RowRumber字段未解析");
            // 2. 解析RowRumber_OrderBy字段
            string columns;

            // 查指定字段 
            NewExpression arryexps = fielambda.Body as NewExpression;
            Dictionary<string, int> pdic = base.GetLmdparamsDic(fielambda);
            columns = GetFieldrrExps(arryexps.Arguments, pdic); //"select " +  //, TabAliaceDic
            columns = string.Format(SM.LimitRowNumber_Sql, columns);
            Clauses.Add(Clause.New(ClauseType.ActionSelectRowRumberOrderBy, rowRumberOrderBy: columns));
            return this;
        }
        /// <summary>
        /// 查询指定字段(默认查询*所有字段) 匿名类型传入Fileds t =>  new { t.f1, t.f2, t2.f3 }   ==>   tab1.f1, tab1.f2, tab2.f3
        /// </summary>
        public DapperSqlMaker<T, Y, Z> Column(Expression<Func<T, Y, Z, object>> fiesExps = null)
        {
            string columns;
            if (fiesExps == null) { columns = SM.ColumnAll; goto columnsall; }

            LambdaExpression fielambda = fiesExps as LambdaExpression;
            columns = base.GetColumnStr(fielambda);

            columnsall:
            Clauses.Add(Clause.New(ClauseType.ActionSelectColumn, selectColumn: columns));
            return this;
        }
        public DapperSqlMaker<T, Y, Z> FromJoin(
            JoinType joinType2, Expression<Func<T, Y, Z, bool>> joinExps2
            , JoinType joinType3, Expression<Func<T, Y, Z, bool>> joinExps3)
        {
            var tabAliasName1 = "a"; //fielambda.Parameters[0].Name;
            var tabAliasName2 = "b"; //fielambda.Parameters[1].Name;
            var tabAliasName3 = "c"; //fielambda.Parameters[2].Name;
                                     //// 1. 存表别名
                                     //var tabAliasName1 = fielambda.Parameters[0].Name;
                                     //var tabAliasName2 = fielambda.Parameters[1].Name;
                                     //var tabAliasName3 = fielambda.Parameters[2].Name;
                                     //TabAliaceDic.Add(typeof(T).FullName + 1, tabAliasName1);
                                     //var tab2fname = typeof(Y).FullName;
                                     //if (!TabAliaceDic.ContainsKey(tab2fname)) TabAliaceDic.Add(tab2fname + 2, tabAliasName2);
                                     //var tab3fname = typeof(Z).FullName;
                                     //if (!TabAliaceDic.ContainsKey(tab3fname)) TabAliaceDic.Add(tab3fname + 3, tabAliasName3);

            // 3. 主表和查询字段 sel ... from tab
            var tabname1 = DsmSqlMapperExtensions.GetTableName(typeof(T));
            var selstr = $" from {tabname1} {tabAliasName1}"; // sel .. from 表明 别名

            // 4. 生成联表2sql join tab on ...
            string jointabstr2 = base.GetJoinTabStr(typeof(Y), tabAliasName2, joinType2, joinExps2);
            // 5. 生成联表3sql
            string jointabstr3 = base.GetJoinTabStr(typeof(Z), tabAliasName3, joinType3, joinExps3);

            Clauses.Add(Clause.New(ClauseType.ActionSelectFrom, fromJoin: selstr + jointabstr2 + jointabstr3));
            return this;
        }

        public DapperSqlMaker<T, Y, Z> Where(Expression<Func<T, Y, Z, bool>> whereExps)
        {
            DynamicParameters spars;
            string sqlCondition;
            base.GetWhereStr(whereExps, out spars, out sqlCondition);

            Clauses.Add(Clause.New(ClauseType.ActionSelectWhereOnHaving, condition: sqlCondition, conditionParms: spars));

            return this;
        }

        /// <param name="fiesExps"></param>
        /// <param name="isDesc">是否降序</param>
        /// <returns></returns>
        public DapperSqlMaker<T, Y, Z> Order(Expression<Func<T, Y, Z, object>> fiesExps, bool isDesc = false)
        {
            LambdaExpression fielambda = fiesExps as LambdaExpression;
            string columns = base.GetOrderStr(fielambda, isDesc);
            Clauses.Add(Clause.New(ClauseType.ActionSelectOrder, order: columns));
            return this;
        }

    }

    public abstract class DapperSqlMaker<T, Y> : DapperSqlMaker
    {
        public abstract override IDbConnection GetConn();

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<dynamic> ExecuteQuery()
        {
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = base.RawSqlParams();

            using (var conn = GetConn())
            {
                var obj = conn.Query<dynamic>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                return obj;
            }
        }

        public DapperSqlMaker<T, Y> Select()
        {
            // 1. 存表序号和表别名 //FromJoin 得用到
            var tabAliasName1 = "a"; //fielambda.Parameters[0].Name;
            var tabAliasName2 = "b"; //fielambda.Parameters[1].Name; 
            TabAliaceDic.Add(typeof(T).FullName + 1, tabAliasName1);
            var tab2fname = typeof(Y).FullName;
            if (!TabAliaceDic.ContainsKey(tab2fname)) TabAliaceDic.Add(tab2fname + 2, tabAliasName2);
            Clauses.Add(Clause.New(ClauseType.ActionSelect, select: " select "));
            return this;
        }
        /// <summary>
        /// MSSQL RowRumber OrderBy 字段  p => p.Id  orderfiesExps必须有返回值 后面的orderby就不要拼接了
        /// </summary> 
        public DapperSqlMaker<T, Y> RowRumberOrderBy(Expression<Func<T, Y, object>> orderfiesExps)
        {
            LambdaExpression fielambda = orderfiesExps as LambdaExpression;
            if (!(fielambda.Body is NewExpression)) throw new Exception("RowRumber字段未解析");
            // 2. 解析RowRumber_OrderBy字段
            string columns;

            // 查指定字段 
            NewExpression arryexps = fielambda.Body as NewExpression;
            Dictionary<string, int> pdic = base.GetLmdparamsDic(fielambda);
            columns = GetFieldrrExps(arryexps.Arguments, pdic); //"select " +//, TabAliaceDic
            columns = string.Format(SM.LimitRowNumber_Sql, columns);
            Clauses.Add(Clause.New(ClauseType.ActionSelectRowRumberOrderBy, rowRumberOrderBy: columns));
            return this;
        }
        /// <summary>
        /// 查询指定字段(默认查询*所有字段) 匿名类型传入Fileds t =>  new { t.f1, t.f2, t2.f3 }   ==>   tab1.f1, tab1.f2, tab2.f3
        /// </summary>
        public DapperSqlMaker<T, Y> Column(Expression<Func<T, Y, object>> fiesExps = null)
        {
            string columns;
            if (fiesExps == null) { columns = SM.ColumnAll; goto columnsall; }

            LambdaExpression fielambda = fiesExps as LambdaExpression;
            columns = base.GetColumnStr(fielambda);

            columnsall:
            Clauses.Add(Clause.New(ClauseType.ActionSelectColumn, selectColumn: columns));
            return this;
        }

        /// <summary>
        /// 查询字段连表  待完善扩展列 分页行号 ????
        /// </summary>
        /// <param name="fiesExps">查询指定字段 栗 (lp, u) => new { t.f1, t.f2, t2.f3 }  Func返回值为空时查询所有字段 </param>
        /// <param name="joinType">表连接类型</param>
        /// <param name="joinExps">表连接条件</param>
        /// <returns></returns>
        public DapperSqlMaker<T, Y> FromJoin(JoinType joinType, Expression<Func<T, Y, bool>> joinExps)
        {
            var tabAliasName1 = "a"; //fielambda.Parameters[0].Name;
            var tabAliasName2 = "b"; //fielambda.Parameters[1].Name;

            // sel ... from tab
            var tabname1 = DsmSqlMapperExtensions.GetTableName(typeof(T));
            var selstr = $" from {tabname1} {tabAliasName1}"; // sel .. from 表明 别名

            // 4. 生成联表2sql join tab on ...
            string jointabstr2 = base.GetJoinTabStr(typeof(Y), tabAliasName2, joinType, joinExps);

            //sqlMaker.
            Clauses.Add(Clause.New(ClauseType.ActionSelectFrom, fromJoin: selstr + jointabstr2));
            return this;//sqlMaker;
        }

        //public DapperSqlMaker<T, Y> JoinTable(JoinType joinType, Expression<Func<T, Y, bool>> joinExps)
        //{
        //    // 主表再select中
        //    //var tabname1 = DsmSqlMapperExtensions.GetTableName(typeof(T));  
        //    var tabname2 = DsmSqlMapperExtensions.GetTableName(typeof(Y));

        //    var joinstr = joinType == JoinType.Inner ? " inner join "
        //                  : joinType == JoinType.Left ? " left join "
        //                  : joinType == JoinType.Right ? " right join "
        //                  : null;

        //    LambdaExpression lambda = joinExps as LambdaExpression;
        //    BinaryExpression binaryg = lambda.Body as BinaryExpression;

        //    MemberExpression Member1 = binaryg.Left as MemberExpression;
        //    MemberExpression Member2 = binaryg.Right as MemberExpression;

        //    ParameterExpression Parmexr1 = Member1.Expression as ParameterExpression;
        //    var mberName1 = Parmexr1.Name + "." + Member1.Member.Name;  // 表(别名).字段名

        //    ParameterExpression Parmexr2 = Member2.Expression as ParameterExpression;
        //    var mberName2 = Parmexr2.Name + "." + Member2.Member.Name;  // 表(别名).字段名

        //    var strJoinTable = $" {joinstr} {tabname2} on {mberName1} = {mberName2} ";

        //    Clauses.Add(Clause.New(ClauseType.ActionSelectJoin, jointable: strJoinTable));

        //    return this;
        //}
        //public DapperSqlMaker<T, Y> LeftJoin(Expression<Func<T, Y, bool>> joinExps)
        //{
        //    this.JoinTable(JoinType.Left, joinExps); 
        //    return this;
        //}
        //public DapperSqlMaker<T, Y> RightJoin(Expression<Func<T, Y, bool>> joinExps)
        //{
        //    this.JoinTable(JoinType.Right, joinExps);
        //    return this;
        //}
        //public DapperSqlMaker<T, Y> InnerJoin(Expression<Func<T, Y, bool>> joinExps)
        //{
        //    this.JoinTable(JoinType.Inner, joinExps);
        //    return this;
        //}

        public DapperSqlMaker<T, Y> Where(Expression<Func<T, Y, bool>> whereExps)
        {
            DynamicParameters spars;
            string sqlCondition;
            base.GetWhereStr(whereExps, out spars, out sqlCondition);

            Clauses.Add(Clause.New(ClauseType.ActionSelectWhereOnHaving, condition: sqlCondition, conditionParms: spars));

            return this;
        }

        // 扩展列 分页行号
        public DapperSqlMaker<T, Y> Order(Expression<Func<T, Y, object>> fiesExps, bool isDesc = false)
        {
            LambdaExpression fielambda = fiesExps as LambdaExpression;
            string columns = base.GetOrderStr(fielambda, isDesc);
            Clauses.Add(Clause.New(ClauseType.ActionSelectOrder, order: columns));
            return this;
        }


    }

    public abstract class DapperSqlMaker<T> : DapperSqlMaker
        where T : class, new()
    {
        public abstract override IDbConnection GetConn();

        #region 链式查询数据

        // 查询
        public DapperSqlMaker<T> Select()
        {
            // 1. 存表序号和表别名 //FromJoin 得用到
            var tabAliasName1 = "a"; //fielambda.Parameters[0].Name; 
            TabAliaceDic.Add(typeof(T).FullName + 1, tabAliasName1);

            Clauses.Add(Clause.New(ClauseType.ActionSelect, select: " select "));
            base.ClauseFirst = ClauseType.ActionSelect;
            return this;
        }
        /// <summary>
        /// MSSQL RowRumber OrderBy 字段  p => p.Id  orderfiesExps必须有返回值 后面的orderby就不要拼接了
        /// </summary> 
        public DapperSqlMaker<T> RowRumberOrderBy(Expression<Func<T, object>> orderfiesExps)
        {
            LambdaExpression fielambda = orderfiesExps as LambdaExpression;
            if (!(fielambda.Body is NewExpression)) throw new Exception("RowRumber字段未解析");
            // 2. 解析RowRumber_OrderBy字段
            string columns;
            // 查指定字段 
            NewExpression arryexps = fielambda.Body as NewExpression;
            Dictionary<string, int> pdic = base.GetLmdparamsDic(fielambda);
            columns = GetFieldrrExps(arryexps.Arguments, pdic); //"select " + //, TabAliaceDic
            columns = string.Format(SM.LimitRowNumber_Sql, columns);
            Clauses.Add(Clause.New(ClauseType.ActionSelectRowRumberOrderBy, rowRumberOrderBy: columns));
            return this;
        }
        /// <summary>
        /// 查询指定字段(默认查询*所有字段) 匿名类型传入Fileds t =>  new { t.f1, t.f2, t2.f3 }   ==>   tab1.f1, tab1.f2, tab2.f3
        /// </summary>
        public DapperSqlMaker<T> Column(Expression<Func<T, object>> fiesExps = null)
        {
            string columns;
            if (fiesExps == null) { columns = SM.ColumnAll; goto columnsall; }

            LambdaExpression fielambda = fiesExps as LambdaExpression;
            columns = base.GetColumnStr(fielambda);

            columnsall:
            Clauses.Add(Clause.New(ClauseType.ActionSelectColumn, selectColumn: columns));
            return this;
        }
        public DapperSqlMaker<T> From()
        {
            // 1. 存表别名 
            var tabAliasName1 = "a";
            // 3. 主表和查询字段 sel ... from tab
            var tabname1 = DsmSqlMapperExtensions.GetTableName(typeof(T));
            var selstr = $" from {tabname1} {tabAliasName1}"; // sel .. from 表明 别名

            Clauses.Add(Clause.New(ClauseType.ActionSelectFrom, fromJoin: selstr));
            return this;
        }
        public DapperSqlMaker<T> Where(Expression<Func<T, bool>> whereExps)
        {
            DynamicParameters spars;
            string sqlCondition;
            base.GetWhereStr(whereExps, out spars, out sqlCondition);

            Clauses.Add(Clause.New(ClauseType.ActionSelectWhereOnHaving, condition: sqlCondition, conditionParms: spars));

            return this;
        }
        public DapperSqlMaker<T> Order(Expression<Func<T, object>> fiesExps, bool isDesc = false)
        {
            LambdaExpression fielambda = fiesExps as LambdaExpression;
            string columns = base.GetOrderStr(fielambda, isDesc);
            Clauses.Add(Clause.New(ClauseType.ActionSelectOrder, order: columns));
            return this;
        }

        #endregion

        #region 链式 添加数据

        /// <summary>
        /// 链式 添加语法
        /// </summary>
        public DapperSqlMaker<T> Insert()
        {
            // 1. insert into tab
            var tabname1 = DsmSqlMapperExtensions.GetTableName(typeof(T));
            Clauses.Add(Clause.New(ClauseType.Insert, insert: " insert into " + tabname1));
            base.ClauseFirst = ClauseType.Insert;
            return this;
            // $"insert into {name} ({sbColumnList}) values ({sbParameterList})"
        }
        public DapperSqlMaker<T> AddColumn(Expression<Func<T, bool[]>> fiesExps = null)
        {
            DynamicParameters spars;
            string sqlColmval;
            if (fiesExps == null) throw new Exception("不能执行空的插入语句");
            LambdaExpression fielambda = fiesExps as LambdaExpression;
            base.GetInsertOrUpdateColumnValueStr(fielambda, out spars, out sqlColmval);
            Clauses.Add(Clause.New(ClauseType.AddColumn, addcolumn: sqlColmval, insertParms: spars));
            return this;
        }
        // Insert 影响行数  Insert 最后插入数据Id

        #endregion

        #region 链式 更新数据
        public DapperSqlMaker<T> Update()
        {
            // 1. insert into tab
            var tabname1 = DsmSqlMapperExtensions.GetTableName(typeof(T));
            Clauses.Add(Clause.New(ClauseType.Update, update: " update " + tabname1));
            base.ClauseFirst = ClauseType.Update;
            return this;
            // $" update {name} set {EditColumn}where {where}"
        }
        public DapperSqlMaker<T> EditColumn(Expression<Func<T, bool[]>> fiesExps = null)
        {
            DynamicParameters spars;
            string sqlColmval;
            if (fiesExps == null) throw new Exception("不能执行空的插入语句");
            LambdaExpression fielambda = fiesExps as LambdaExpression;
            base.GetInsertOrUpdateColumnValueStr(fielambda, out spars, out sqlColmval, addOrEdit: 2);
            Clauses.Add(Clause.New(ClauseType.EditColumn, editcolumn: " set " + sqlColmval, updateParms: spars));

            return this;
            // $"insert into {name} ({sbColumnList}) values ({sbParameterList})"
        }

        #endregion

        #region 链式 删除数据
        public DapperSqlMaker<T> Delete()
        {
            // 1. insert into tab
            var tabname1 = DsmSqlMapperExtensions.GetTableName(typeof(T));
            // 添加where关键字防止全表删除
            Clauses.Add(Clause.New(ClauseType.Delete, delete: string.Format(" delete from {0} where ", tabname1)));
            base.ClauseFirst = ClauseType.Delete;
            return this;
            // $" delete from {0} where "
        }



        #endregion

    }
    public abstract class DapperSqlMaker
    {
        public abstract IDbConnection GetConn();
        public DapperSqlMaker()
        {
            GetConn().Dispose(); //在子类必须重写抽象方法
        }
        // 当前连接
        //protected abstract IDbConnection GetConn();

        private static readonly ISqlAdapter DefaultAdapter = new SqlServerAdapter();
        private static readonly Dictionary<string, ISqlAdapter> AdapterDictionary
            = new Dictionary<string, ISqlAdapter>
            {
                {"sqlconnection", new SqlServerAdapter()},
                {"sqlceconnection", new SqlCeServerAdapter()},
                {"npgsqlconnection", new PostgresAdapter()},
                {"sqliteconnection", new SQLiteAdapter()},
                {"mysqlconnection", new MySqlAdapter()},
            };
        protected ISqlAdapter GetSqlAdapter(IDbConnection connection)
        {
            var name = connection.GetType().Name.ToLower();
            return !AdapterDictionary.ContainsKey(name)
                 ? DefaultAdapter
                 : AdapterDictionary[name];
        }

        protected ClauseType ClauseFirst { get; set; }

        protected enum ClauseType
        {
            ActionSelect,
            //ActionSelectLimitCounts,   
            ActionSelectRowRumberOrderBy,
            ActionSelectColumn,
            ActionSelectFrom,
            //ActionSelectJoin,
            //ActionLeftJoin,
            //ActionRightJoin,
            //ActionInnerJoin,
            ActionSelectWhereOnHaving,
            ActionSelectOrder,
            Table,

            Insert,
            AddColumn,
            Update,
            EditColumn,
            Delete,
        }

        protected class Clause
        {
            public static Clause New(ClauseType type, string select = null
                , string rowRumberOrderBy = null  //, string selectCounts = null
                , string selectColumn = null, string fromJoin = null
                , string seletTable = null//, string jointable = null, string aliace = null
                , string condition = null, DynamicParameters conditionParms = null
                , string order = null, string extra = null
                , string insert = null, string addcolumn = null, DynamicParameters insertParms = null
                , string update = null, string editcolumn = null, DynamicParameters updateParms = null
                , string delete = null)
            {
                return new Clause
                {
                    ClauseType = type,
                    Select = select,
                    //SelectCounts = selectCounts,
                    SeletTable = seletTable, // 无用
                    RowRumberOrderBy = rowRumberOrderBy,
                    SelectColumn = selectColumn,
                    FromJoin = fromJoin,
                    //JoinTable = jointable,
                    //Aliace = aliace,
                    Condition = condition,
                    ConditionParms = conditionParms,
                    Order = order,
                    Extra = extra,
                    //添加 ------------
                    Insert = insert,
                    AddColumn = addcolumn,
                    InsertParms = insertParms,
                    //修改 ------------
                    Update = update,
                    EditColumn = editcolumn,
                    UpdateParms = updateParms,
                    Delete = delete
                };
            }

            public ClauseType ClauseType { get; private set; }
            public string SeletTable { get; private set; }
            public string Select { get; private set; }
            //public string SelectCounts { get; private set; }
            public string RowRumberOrderBy { get; private set; }
            public string SelectColumn { get; private set; }
            public string FromJoin { get; private set; }
            //public string JoinTable { get; private set; }//
            public string Condition { get; private set; } // where
            public string Order { get; private set; }
            //public string Aliace { get; private set; } 
            public DynamicParameters ConditionParms { get; private set; }
            public string Extra { get; private set; } // 字段 
            public string Insert { get; private set; }
            public string AddColumn { get; private set; }
            public DynamicParameters InsertParms { get; private set; }
            public string Update { get; private set; }
            public string EditColumn { get; private set; }
            public DynamicParameters UpdateParms { get; private set; }
            public string Delete { get; private set; }
        }

        //protected class TabAliace {
        //    public static Dictionary<string, string> Dic = new Dictionary<string, string>();
        //    //public static TabAliace New(string name, string aliace) {
        //    //    return new TabAliace { Name = name,Aliace = aliace };
        //    //}
        //    //public string Name { get; private set; }
        //    //public string Aliace { get; private set; } 
        //}

        protected List<Clause> _clauses;
        protected List<Clause> Clauses
        {
            get { return _clauses ?? (_clauses = new List<Clause>()); }
        }
        //表别名 ConcurrentDictionary FullName, tabAliasName

        protected Dictionary<string, string> _TabAliace;
        /// <summary>
        /// 类全名 + 序号  表别名已经改为按顺序a,b,c,d的形式 FromJoin方法需要使用 where不用了 ？？？？ 看要不要把FromJoin改了
        /// </summary>
        protected Dictionary<string, string> TabAliaceDic
        {
            get { return _TabAliace ?? (_TabAliace = new Dictionary<string, string>()); }
        }

        #region 解析 查询sql
        protected Dictionary<string, int> GetLmdparamsDic(LambdaExpression fielambda)
        {
            Dictionary<string, int> pdic = new Dictionary<string, int>();
            int i = 1;
            foreach (var p in fielambda.Parameters)
            {
                pdic.Add(p.Name, i++);
            }

            return pdic;
        }

        /// <summary>
        /// 生成字段 tab1.Field1, tab1.Field2, tab2.Field3
        /// </summary> 
        /// <param name="fierrExps"></param>
        /// <param name="paramsdic">lmb参数名 序号 字典</param>
        /// <returns></returns>
        protected string GetFieldrrExps(System.Collections.ObjectModel.ReadOnlyCollection<Expression> fierrExps, Dictionary<string, int> paramsdic
            , string[] asnameArr = null) // , System.Collections.ObjectModel.ReadOnlyCollection<System.Reflection.MemberInfo> nmebs = null)
        {

            int i = 0; // 成员序号
            string columns;
            var fiearr = fierrExps.Select(p =>
            {
                bool isfield_suf = asnameArr != null;
                string colum = null;   // 字段
                string field_suf = null; // 字段后缀 (as xxx , desc)
                MemberExpression meb;
                ParameterExpression parmexr;


                if (p.NodeType == ExpressionType.MemberAccess)
                {
                    meb = p as MemberExpression; goto mebexpisnull;
                }
                if (p.NodeType == ExpressionType.Constant)
                {
                    ConstantExpression const1 = p as ConstantExpression;
                    if (p.Type.Name.ToLower() != "string") throw new Exception(p.Type.Name + "常量未解析");
                    colum = const1.Value.ToString();
                    isfield_suf = false; // 直接写sql 别名也要写在字符串里
                    goto appendsql;
                }
                if (p.NodeType == ExpressionType.Convert)
                {
                    UnaryExpression umeb = p as UnaryExpression;
                    meb = umeb.Operand as MemberExpression;
                    goto mebexpisnull; //Constant;
                }
                //;

                if (p.NodeType == ExpressionType.Call)
                {
                    MethodCallExpression method = p as MethodCallExpression;
                    if (method.Method.Name == SM._OrderDesc_Name)
                    { // 倒序字段 order by xx desc
                        meb = method.Arguments[0] as MemberExpression;
                        field_suf = SM.OrderDesc_Sql;
                        goto callstr;
                    }
                    else if (method.Method.Name == SM._Sql_Name) // 插入sql 过时 上面直接判断是否时常量就是
                    {
                        meb = method.Arguments[0] as MemberExpression;
                        if (method.Arguments.FirstOrDefault() is ConstantExpression)
                        { //
                            colum = (method.Arguments.FirstOrDefault() as ConstantExpression).Value.ToString();
                        }
                        else if (method.Arguments.FirstOrDefault() is MemberExpression)
                        { // 值 传入的时 变量 

                            colum = AnalysisExpression.GetMemberValue(method.Arguments.FirstOrDefault() as MemberExpression).ToString();
                        }
                        else throw new Exception(SM._OrderDesc_Name + "未解析");
                        isfield_suf = false; // 直接写sql 别名也要写在字符串里
                        goto appendsql;

                    }
                    else throw new Exception(method.Method.Name + "未解析");
                    //Arguments
                }
                else throw new Exception(p.NodeType + "--" + p + "未解析");



                mebexpisnull:
                if (meb.Expression == null)
                { // 特殊sql
                    string fname = meb.Member.DeclaringType.Name + "." + meb.Member.Name;
                    if (fname == SM._limitcount_Name)
                    {
                        colum = SM.LimitCount;  // 分页Count总记录数字段
                    }
                    else throw new Exception(fname + "未解析");
                    isfield_suf = false; // 已定义的特殊sql 也忽略别名
                    goto appendsql;
                }

                callstr: // 方法直接到这

                parmexr = meb.Expression as ParameterExpression;
                //var tkey = parmexr.Type.FullName + paramsdic[parmexr.Name];  // 类名+参数序号
                //var tabalias1 = tabalis[tkey]; // Parmexr1.Name
                //colum = tabalias1 + "." + meb.Member.Name;  // 表(别名).字段名

                colum = ((char)(paramsdic[parmexr.Name] + 96)) + "." + meb.Member.Name;  // 表(别名).字段名

                appendsql: // 字段名 加  后缀(as xxx, desc)

                if (isfield_suf) field_suf = " as " + asnameArr[i]; // 字段别名
                i++;
                return colum + field_suf;
            }).ToArray<string>();
            columns = " " + string.Join(", ", fiearr) + " ";
            return columns;
        }
        /// <summary>
        /// 生成查询 字段列 
        /// </summary>
        /// <returns></returns>
        protected string GetColumnStr(LambdaExpression fielambda)
        {
            string columns;
            // 2. 解析查询字段
            if (fielambda.Body is NewExpression)
            { // 查询指定字段  // 匿名类型传入Fileds   new { t.f1, t.f2, t2.f3 }   ==>   tab1.f1, tab1.f2, tab2.f3
                Dictionary<string, int> pdic = GetLmdparamsDic(fielambda); //base.
                NewExpression arryexps = fielambda.Body as NewExpression;

                var asnameArr = arryexps.Members.Select(m => m.Name).ToArray<string>();

                columns = GetFieldrrExps(arryexps.Arguments, pdic, asnameArr); //
            }
            else columns = SM.ColumnAll; // 查询所有字段

            return columns;
        }
        /// <summary>
        /// 生成联表 sql
        /// </summary>
        /// <param name="joinType2">联表方式</param>
        /// <param name="tabAliasName2">联表别名</param>
        /// <param name="joinlambda">联表条件表达式</param>
        /// <param name="jointab">联表实体类型</param>
        /// <returns></returns>
        protected string GetJoinTabStr(Type jointab, string tabAliasName2, JoinType joinType2, LambdaExpression joinlambda)
        {
            var joinstr = joinType2 == JoinType.Inner ? " inner join "
                                      : joinType2 == JoinType.Left ? " left join "
                                      : joinType2 == JoinType.Right ? " right join "
                                      : null;

            string tabname2 = DsmSqlMapperExtensions.GetTableName(jointab) + " " + tabAliasName2;  // 表名 别名

            StringBuilder sql = null;
            DynamicParameters spars = null;

            int aliasIndex = 1;
            Dictionary<string, int> paramsdic = new Dictionary<string, int>();
            foreach (var p in joinlambda.Parameters)
            {
                paramsdic.Add(p.Name, aliasIndex++);
            }

            AnalysisExpression.JoinExpression(joinlambda, ref sql, ref spars, paramsdic: paramsdic); //sqlMaker.TabAliasName
            string onCondition = sql.ToString();
            var jointabstr = $" {joinstr} {tabname2} on {onCondition} ";

            //BinaryExpression binaryg = joinlambda.Body as BinaryExpression;

            //MemberExpression Member1 = binaryg.Left as MemberExpression;
            //MemberExpression Member2 = binaryg.Right as MemberExpression;

            //ParameterExpression Parmexr1 = Member1.Expression as ParameterExpression;
            //var tabalias1 = TabAliaceDic[Parmexr1.Type.FullName]; //sqlMaker.TabAliasName Parmexr1.Name
            //var mberName1 = tabalias1 + "." + Member1.Member.Name;  // 表(别名).字段名

            //ParameterExpression Parmexr2 = Member2.Expression as ParameterExpression;
            //var tabalias2 = TabAliaceDic[Parmexr2.Type.FullName]; //sqlMaker.TabAliasName Parmexr2.Name
            //var mberName2 = tabalias2 + "." + Member2.Member.Name;  // 表(别名).字段名

            //var jointabstr = $" {joinstr} {tabname2} on {mberName1} = {mberName2} ";
            return jointabstr;
        }
        /// <summary>
        /// 生成order sql
        /// </summary>
        /// <param name="fielambda">排序字段 (lp, w) => new { lp.EditCount, lp.Name }</param>
        /// <param name="isDesc">是否降序</param>
        /// <returns></returns>
        protected string GetOrderStr(LambdaExpression fielambda, bool isDesc)
        {

            Dictionary<string, int> pdic = this.GetLmdparamsDic(fielambda);
            string columns;
            NewExpression newexps = fielambda.Body as NewExpression;
            columns = " order by " + GetFieldrrExps(newexps.Arguments, pdic) + (isDesc ? " desc " : null); //base.TabAliasName

            return columns;
        }
        /// <summary>
        /// 生成where sql
        /// </summary>
        /// <param name="whereExps">查询条件(lp, u) => lp.Name == lpmodel.Name || u.UserName == umodel.UserName</param>
        /// <param name="spars">查询条件参数</param>
        /// <param name="sqlCondition">查询条件sql</param>
        protected void GetWhereStr(Expression whereExps, out DynamicParameters spars, out string sqlCondition)
        {
            StringBuilder sql = null;
            spars = null;
            // select子句有表别名  update子句不能有表别名

            AnalysisExpression.JoinExpression(whereExps, ref sql, ref spars, isAliasName: this.ClauseFirst == ClauseType.ActionSelect); //sqlMaker.TabAliasName

            // 防止 update和delete全表操作 已在前面添加where关键字   只有查询语句会执行下行
            if (this.ClauseFirst != ClauseType.Delete && this.ClauseFirst != ClauseType.Update)
                sql.Insert(0, sql.Length > 0 ? " where " : " ");
            sqlCondition = sql.ToString();
        }

        #endregion

        #region 解析插入语句sql
        /// <summary>
        /// 生成插入语句 Columns Values
        /// </summary>
        /// <param name="colmvalambda">列和值语法规范p => new object[] { p.Content == p.Name,p.IsDel == false } </param>
        /// <param name="spars">插入语句参数</param>
        /// <param name="sqlColmval">插入语句sql</param>
        /// <param name="addOrEdit">默认新增 1 新增 2 修改</param>
        protected void GetInsertOrUpdateColumnValueStr(LambdaExpression colmvalambda, out DynamicParameters spars, out string sqlColmval, int addOrEdit = 1)
        {
            //2.解析查询字段
            if (!(colmvalambda.Body is NewArrayExpression)) throw new Exception("不能执行空的插入语句");
            // 列和值语法规范p => new object[] { p.Content == p.Name,p.IsDel == false }   ==>   (Content,IsDel) Value(@Content,@IsDel)
            NewArrayExpression arryexps = colmvalambda.Body as NewArrayExpression;
            StringBuilder sb = new StringBuilder();
            spars = new Dapper.DynamicParameters();
            List<string[]> customColmval = new List<string[]>();
            var lmbdParmName = colmvalambda.Parameters[0].Name;
            int num = 1;
            string exgl = "=";
            if (addOrEdit == 1) sb.Append(" ( "); // 添加输出 修改不用
            foreach (var p in arryexps.Expressions)
            {
                if (p.NodeType == System.Linq.Expressions.ExpressionType.Equal && addOrEdit == 1)
                { //添加
                    AnalysisExpression.BinaryExprssRowSqlParms(p, sb, spars, num++, exgl, (name, parmasName, exglstr) => string.Format("{0},", name)); //" {0} {2} @{0}{1} "
                }
                else if (p.NodeType == System.Linq.Expressions.ExpressionType.Equal && addOrEdit == 2)
                { // 修改
                    AnalysisExpression.BinaryExprssRowSqlParms(p, sb, spars, num++, exgl, (name, parmasName, exglstr) => string.Format(" {0}{2}@{1} ,", name, parmasName, exglstr)); //" {0} {2} @{0}{1} "
                }
                else if (p.NodeType == System.Linq.Expressions.ExpressionType.Call)
                {
                    System.Linq.Expressions.MethodCallExpression method = p as System.Linq.Expressions.MethodCallExpression;

                    if (method.Method.Name != SM._Sql_Name) throw new Exception(method.Method.Name + " 暂未做解析的方法 " + p);

                    string[] arrColmval = new string[2]; // 0 column  1 value
                    int i = 0;
                    tempstart:
                    //meb = method.Arguments[0] as System.Linq.Expressions.MemberExpression;
                    if (method.Arguments[i] is System.Linq.Expressions.ConstantExpression)
                    {
                        // 参数名/插入值 直接赋值
                        arrColmval[i] = (method.Arguments[i] as System.Linq.Expressions.ConstantExpression).Value.ToString();
                    }
                    else if (method.Arguments[i] is System.Linq.Expressions.MemberExpression)
                    {// 参数名/插入值 传入的时 变量 

                        var meb = method.Arguments[i] as System.Linq.Expressions.MemberExpression;

                        if (meb.Expression is System.Linq.Expressions.ParameterExpression
                                 && (meb.Expression as System.Linq.Expressions.ParameterExpression).Name == lmbdParmName)
                        { // lambda表达式的参数成员 表示字段参数名 只取成员名称不取值
                            arrColmval[i] = (method.Arguments[i] as System.Linq.Expressions.MemberExpression).Member.Name;
                        }
                        else
                        {// 外部传入的变量
                            arrColmval[i] = AnalysisExpression.GetMemberValue(method.Arguments[i] as System.Linq.Expressions.MemberExpression).ToString();
                        }
                    }
                    //(method.Arguments[i] as System.Linq.Expressions.MemberExpression).Member.Name;
                    else throw new Exception(p.ToString() + " 插入语句未能解析");

                    if (++i < 2) goto tempstart;

                    customColmval.Add(arrColmval);
                }
                else throw new Exception(p.ToString() + " 插入语句未能解析");


            }
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);

            if (addOrEdit == 1)
            { // 添加
                // 拼接子查询插入的 参数名
                sb.Append((spars.ParameterNames.Count() > 0 && customColmval.Count > 0 ? ", " : string.Empty) + string.Join(",", customColmval.Select(p => p[0]).ToList<string>()));

                // 简单参数值 和 子查询
                sb.AppendFormat(") values ({0}{1}) ", string.Join(",", spars.ParameterNames.ToList<string>().Select(p => "@" + p).ToList<string>())
                     , (spars.ParameterNames.Count() > 0 && customColmval.Count > 0 ? ", " : string.Empty) + string.Join(",", customColmval.Select(p => p[1]).ToList<string>()));
            }
            else if (addOrEdit == 2)
            { // 修改
                // 拼接子查询插入的 参数名
                sb.Append((spars.ParameterNames.Count() > 0 && customColmval.Count > 0 ? ", " : string.Empty) + string.Join(",", customColmval.Select(p => p[0] + "=" + p[1]).ToList<string>()));
                sb.Append(" where "); // 添加where关键字防止全表操作
                // 简单参数值 和 子查询
                //sb.AppendFormat(") val/*ues ({0}{1}) ", string.Join(",", spars.ParameterNames.ToList<string>().Select(p => "@" + p).ToList<string>())*/
                //     , (spars.ParameterNames.Count() > 0 && customColmval.Count > 0 ? ", " : string.Empty) + string.Join(",", customColmval.Select(p => p[1]).ToList<string>()));
            }

            sqlColmval = sb.ToString();
        }

        #endregion

        protected static DapperSqlMaker _sqlMaker;
        //public static DapperSqlMaker New()
        //{ 
        //    //Clauses.Clear();
        //    _sqlMaker = new DapperSqlMaker();
        //    _sqlMaker.Clauses.Clear();
        //    return _sqlMaker;
        //}

        //public DapperSqlMaker SELECT(string columns = null)
        //{
        //    //Clauses.Add(Clause.New(ClauseType.ActionSelect, "SELECT"));
        //    return this;
        //}

        //public virtual string RawSql()
        //{
        //    string sqlResult = null;
        //    if (Clauses.Count == 0)
        //    {
        //        throw new Exception("Empty query");
        //    }
        //    var first = Clauses.First();
        //    switch (first.ClauseType)
        //    { 
        //        case ClauseType.ActionSelect:
        //            sqlResult = ResolveSelect(Clauses);
        //            break; 
        //        default:
        //            throw new Exception("Wrong start of query");
        //    }
        //    return sqlResult;
        //}

        protected static string ResolveSelect(List<Clause> list)
        {
            var sb = new StringBuilder();
            foreach (var clause in list)
            {
                switch (clause.ClauseType)
                {
                    case ClauseType.ActionSelect:
                        sb.Append(clause.SeletTable);
                        break;
                    //case ClauseType.ActionSelectJoin:
                    //    sb.Append(clause.JoinTable);
                    //    break; 
                    case ClauseType.ActionSelectWhereOnHaving:
                        sb.Append(clause.Condition);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            sb.Append(";");
            return sb.ToString();
        }



        /// <summary>
        /// 生成sql和参数
        /// </summary>
        /// <returns>item1 :sql, item2: 动态参数</returns>
        public Tuple<StringBuilder, DynamicParameters> RawSqlParams()
        {

            if (Clauses.Count == 0)
            {
                throw new Exception("Empty query");
            }
            //var first = Clauses.First();
            if (this.ClauseFirst != ClauseType.ActionSelect && this.ClauseFirst != ClauseType.Insert && this.ClauseFirst != ClauseType.Update && this.ClauseFirst != ClauseType.Delete)
            {
                throw new Exception("Wrong start of query or insert");
            }
            DynamicParameters dparam = new DynamicParameters();
            List<string> columns = new List<string>();
            var sb = new StringBuilder();
            foreach (var clause in Clauses)
            {
                switch (clause.ClauseType)
                {
                    case ClauseType.ActionSelect: // 查询 ----------------
                        sb.Append(clause.Select);
                        break;
                    case ClauseType.ActionSelectRowRumberOrderBy:
                        sb.Append(clause.RowRumberOrderBy);
                        break;
                    case ClauseType.ActionSelectColumn:
                        sb.Append(clause.SelectColumn);
                        break;
                    case ClauseType.ActionSelectFrom:
                        sb.Append(clause.FromJoin);
                        break;
                    case ClauseType.ActionSelectWhereOnHaving:
                        sb.Append(clause.Condition);
                        dparam.AddDynamicParams(clause.ConditionParms);
                        //if (this.ClauseFirst == ClauseType.ActionSelect)
                        //    dparam.AddDynamicParams(clause.ConditionParms);
                        //else if (this.ClauseFirst == ClauseType.Update)
                        //    dparam.AddDynamicParams(clause.ConditionParms);
                        //else if (this.ClauseFirst == ClauseType.Delete)
                        //    dparam.AddDynamicParams(clause.ConditionParms);
                        break;
                    case ClauseType.ActionSelectOrder:
                        sb.Append(clause.Order);
                        break; // --------------查询

                    case ClauseType.Insert: // 新增 -----------------------
                        sb.Append(clause.Insert);
                        break;
                    case ClauseType.AddColumn:
                        sb.Append(clause.AddColumn);
                        dparam.AddDynamicParams(clause.InsertParms);
                        break;// ----------新增

                    case ClauseType.Update: // 更新 -----------------------
                        sb.Append(clause.Update);
                        break;
                    case ClauseType.EditColumn:
                        sb.Append(clause.EditColumn);
                        dparam.AddDynamicParams(clause.UpdateParms);
                        break;// ----------更新 where子句公用select的
                    case ClauseType.Delete: // 删除 -------------------
                        sb.Append(clause.Delete); 
                        break;// ----------删除 where子句公用select的
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            //sb.Append(";");
            if (dparam == null) dparam = new DynamicParameters(); // where 条件为空
            return Tuple.Create(sb, dparam);
        }


        /// <summary>
        /// 值1 select * from ...  值3 from ...  
        /// </summary>
        /// <returns></returns>
        public Tuple<StringBuilder, DynamicParameters, StringBuilder> RawLimitSqlParams()
        {

            if (Clauses.Count == 0)
            {
                throw new Exception("Empty query");
            }
            var first = Clauses.First();
            if (first.ClauseType != ClauseType.ActionSelect)
            {
                throw new Exception("Wrong start of query");
            }
            DynamicParameters dparam = null;
            var sb = new StringBuilder();
            var countsb = new StringBuilder();
            foreach (var clause in Clauses)
            {
                switch (clause.ClauseType)
                {
                    case ClauseType.ActionSelect:
                        sb.Append(clause.Select);
                        //countsb.Append(clause.Select);
                        break;
                    case ClauseType.ActionSelectColumn:
                        sb.Append(clause.SelectColumn);  // 查询分页数据  
                        break;
                    case ClauseType.ActionSelectFrom:
                        sb.Append(clause.FromJoin);
                        countsb.Append(clause.FromJoin);
                        break;
                    case ClauseType.ActionSelectWhereOnHaving:
                        sb.Append(clause.Condition);
                        countsb.Append(clause.Condition);
                        dparam = clause.ConditionParms;
                        break;
                    case ClauseType.ActionSelectOrder:
                        sb.Append(clause.Order);
                        countsb.Append(clause.Order);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            //sb.Append(";");
            if (dparam == null) dparam = new DynamicParameters(); // where 条件为空
            return Tuple.Create(sb, dparam, countsb);
        }

        // 数据库类型 conn.GetType().Name.ToLower()  sqliteconnection
        // "sqlconnection", "sqlceconnection","npgsqlconnection","sqliteconnection","mysqlconnection",


        #region 输出sql执行

        /// <summary>
        /// 查询 
        /// </summary>
        public virtual IEnumerable<dynamic> ExecuteQuery()
        {
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = this.RawSqlParams();

            using (var conn = GetConn())
            {
                var obj = conn.Query<dynamic>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                return obj;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        public virtual IEnumerable<Y> ExecuteQuery<Y>()
        {
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = this.RawSqlParams();

            using (var conn = GetConn())
            {
                var obj = conn.Query<Y>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                return obj;
            }
        }
        /// <summary>
        /// 查询首行
        /// </summary>
        public virtual Y ExecuteQueryFirst<Y>()
        {
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = this.RawSqlParams();

            using (var conn = GetConn())
            {
                var obj = conn.Query<Y>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                return obj.FirstOrDefault();
            }
        }
        /// <summary>
        /// 查询首列
        /// </summary>
        public virtual Y ExecuteScalar<Y>()
        {
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = this.RawSqlParams();

            using (var conn = GetConn())
            {
                var obj = conn.Query<Y>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                return obj.FirstOrDefault();
            }
        }


        /// <summary>
        /// mssql分页
        /// </summary>  
        public virtual IEnumerable<dynamic> LoadPagems(int page, int rows, out int records)
        {
            records = 0;
            ISqlAdapter adp;
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = this.RawSqlParams();
            using (var conn = GetConn())
            {
                adp = GetSqlAdapter(conn);

                // 生成分页sql
                adp.RawPage(rawSqlParams.Item1, rawSqlParams.Item2, page, rows);
                // 查询分页数据
                var obj = conn.Query<dynamic>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                var first = obj.FirstOrDefault();
                if (first != null) records = int.Parse("0" + first.counts);
                return obj;
            }
        }
        /// <summary>
        /// mssql分页 T实体里声明records接受总记录数;
        /// </summary>  
        public virtual IEnumerable<T> LoadPagems<T>(int page, int rows)
        {
            ISqlAdapter adp;
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = this.RawSqlParams();
            using (var conn = GetConn())
            {
                adp = GetSqlAdapter(conn);

                // 生成分页sql
                adp.RawPage(rawSqlParams.Item1, rawSqlParams.Item2, page, rows);
                // 查询分页数据
                var obj = conn.Query<T>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                //var first = obj.FirstOrDefault();
                //if (first != null) records = int.Parse("0" + first.counts);
                return obj;
            }
        }

        /// <summary>
        /// sqlite分页
        /// </summary> 
        public virtual IEnumerable<dynamic> LoadPagelt(int page, int rows, out int records)
        {
            ISqlAdapter adp;
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters, StringBuilder> rawSqlParams = this.RawLimitSqlParams();
            using (var conn = GetConn())
            {
                adp = GetSqlAdapter(conn);

                adp.RawPage(rawSqlParams.Item1, rawSqlParams.Item2, page, rows);
                // 查询分页数据
                var obj = conn.Query<dynamic>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);

                rawSqlParams.Item3.Insert(0, SM.LimitSelectCount);
                var objrecords = conn.ExecuteScalar(rawSqlParams.Item3.ToString(), rawSqlParams.Item2);

                // 查询总记录数
                //Clauses.Insert(0, Clause.New(ClauseType.ActionSelectColumn, selectColumn: ));
                records = int.Parse(objrecords.ToString());
                return obj;
            }
        }
        /// <summary>
        /// sqlite分页
        /// </summary> 
        public virtual IEnumerable<T> LoadPagelt<T>(int page, int rows, out int records)
        {
            ISqlAdapter adp;
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters, StringBuilder> rawSqlParams = this.RawLimitSqlParams();
            using (var conn = GetConn())
            {
                adp = GetSqlAdapter(conn);

                adp.RawPage(rawSqlParams.Item1, rawSqlParams.Item2, page, rows);
                // 查询分页数据
                var obj = conn.Query<T>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);

                rawSqlParams.Item3.Insert(0, SM.LimitSelectCount);
                var objrecords = conn.ExecuteScalar(rawSqlParams.Item3.ToString(), rawSqlParams.Item2);

                // 查询总记录数
                //Clauses.Insert(0, Clause.New(ClauseType.ActionSelectColumn, selectColumn: ));
                records = int.Parse(objrecords.ToString());
                return obj;
            }
        }


        /// <summary>
        /// 执行添加sql  返回影响行
        /// </summary>
        public virtual int ExecuteInsert()
        {
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = this.RawSqlParams();

            using (var conn = GetConn())
            {
                var obj = conn.Execute(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                return obj;
            }
        }
        /// <summary>
        /// 执行修改sql 返回影响行 修改全表操作需写Where条件  
        /// </summary>
        /// <returns></returns>
        public virtual int ExecuteUpdate() => this.ExecuteInsert();
        /// <summary>
        /// 执行删除sql 返回影响行 删除全表操作需写Where条件  
        /// </summary>
        public virtual int ExecuteDelete() => this.ExecuteInsert();



        #endregion



        //public virtual IEnumerable<dynamic> ExecuteQuery() { throw new Exception("子类未重写该方法"); }

    }


}
