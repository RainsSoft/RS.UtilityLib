using System;
using System.Collections.Generic;
using System.Text;

namespace RS.UtilityLib.WinFormCommon.UISamples
{

    //#region 场地基础数据 BattleFieldInfo 可共用

    ///// <summary>
    ///// 场地基础数据 不直接使用
    ///// </summary>
    //public class BattleFieldInfo {
    //    /// <summary>
    //    /// id 前两位表示房间号 后两位表示场地号
    //    /// 所以一个大厅房间数不超过255，一房间的场地数不超过255
    //    /// </summary>
    //    public int Id { get; set; }
    //    /// <summary>
    //    /// 是否属于webroom ( this.Id == int.MaxValue) 
    //    /// 因为webroom是本地虚构的，其对应的场地也是虚构的
    //    /// </summary>
    //    public bool OwerRoomIsWeb { 
    //        get {
    //            return this.Id == int.MaxValue;
    //        }                 
    //    }
    //    /// <summary>
    //    /// 位置列表 [对象已实例化]
    //    /// </summary>
    //    public List<SeatInfo> Seats = new List<SeatInfo>();
    //    /// <summary>
    //    /// 有人的位置序号列表 [对象已实例化]
    //    /// </summary>
    //    public List<byte> SeatHasPlayer = new List<byte>();// 
    //    //{
    //    //    get {
    //    //        List<byte> m_SeatHasP = new List<byte>();
    //    //        foreach (SeatInfo si in Seats) {
    //    //            if (si.HasUser) {
    //    //                m_SeatHasP.Add(si.Index);
    //    //            }
    //    //        }
    //    //        return m_SeatHasP;
    //    //    }
    //    //}
    //    public BFieldStatus Status = BFieldStatus.Free;

    //}

    //#endregion
    ///// <summary>
    ///// 场地状态
    ///// </summary>
    //public enum BFieldStatus : byte {
    //    /// <summary>
    //    /// 自由 
    //    /// </summary>
    //    Free = 0,
    //    /// <summary>
    //    /// 有人进入
    //    /// </summary>
    //    HasSomeOne = 1,
    //    /// <summary>
    //    /// 准备开始 暂时不用
    //    /// </summary>
    //    ReadyForBattle = 2,
    //    /// <summary>
    //    /// 游戏进行中
    //    /// </summary>
    //    InGame = 3,
    //    /// <summary>
    //    /// 游戏人数不够,等待结束
    //    /// </summary>
    //    ReadyQuit=4,

    //    /// <summary>
    //    /// 游戏加载
    //    /// </summary>
    //    GameLoading=5,
    //}
    //public class SeatInfo {

    //    /// <summary>
    //    /// 从0开始 0-254，
    //    /// </summary>
    //    public byte Index;
    //    /// <summary>
    //    /// 用户id
    //    /// </summary>
    //    public int UserId;
    //    /// <summary>
    //    /// 已经有人坐下
    //    /// </summary>
    //    public bool HasUser {
    //        get {
    //            return UserId != 0;
    //        }
    //    }
    //    public SeatInfo() { }
    //    public SeatInfo(byte index) {
    //        Index = index;
    //    }
    //}

    //public struct MessionType
    //{
    //    /// <summary>
    //    /// 单人提升等级 1
    //    /// </summary>
    //    public const byte LEVEL = 1;
    //    /// <summary>
    //    /// 单人获取积分 2
    //    /// </summary>
    //    public const byte SCCORE = 2;
    //    /// <summary>
    //    /// 同场对抗 3
    //    /// </summary>
    //    public const byte V11 = 3;
    //    /// <summary>
    //    /// 同场对抗 4
    //    /// </summary>
    //    public const byte V22 = 4;
    //    /// <summary>
    //    /// 同场对抗 5
    //    /// </summary>
    //    public const byte V44 = 5;
    //    /// <summary>
    //    ///同场协同 8
    //    /// </summary>
    //    public const byte Co = 8;
    //    /// <summary>
    //    /// 非同场对抗 7
    //    /// </summary>
    //    public const byte VS_Free = 7;//参考连连看
    //    /// <summary>
    //    /// 随便玩 6
    //    /// </summary>
    //    public const byte FREE = 6;
    //}
}
