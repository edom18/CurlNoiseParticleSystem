using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Xorshift
{
    private uint[] _vec = new uint[4];

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="seed">初期シード値</param>
    public Xorshift(uint seed = 100)
    {
        for (uint i = 1; i <= 4; i++)
        {
            seed = 1812433253 * (seed ^ (seed >> 30)) + i;
            _vec[i - 1] = seed;
        }
    }

    /// <summary>
    /// ランダムな値を取得する
    /// </summary>
    /// <returns></returns>
    public float Random()
    {
        uint t = _vec[0];
        uint w = _vec[3];

        _vec[0] = _vec[1];
        _vec[1] = _vec[2];
        _vec[2] = w;

        t ^= t << 11;
        t ^= t >> 8;
        w ^= w >> 19;
        w ^= t;

        _vec[3] = w;

        return w * 2.3283064365386963e-10f;
    }
}
