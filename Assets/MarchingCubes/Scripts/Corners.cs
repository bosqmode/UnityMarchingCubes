using System;

/// <summary>
/// //              v4_______e4_____________v5
//                  /|                    /|
//                 / |                   / |
//              e7/  |                e5/  |
//               /___|______e6_________/   |
//            v7|    |                 |v6 |e9
//              |    |                 |   |
//              |    |e8               |e10|
//           e11|    |                 |   |
//              |    |_________________|___|
//              |   / v0      e0       |   /v1
//              |  /                   |  /
//              | /e3                  | /e1
//              |/_____________________|/
//              v3         e2          v2
/// </summary>

[Flags]
public enum Corners
{
    None = 0,
    v0 = 1,
    v1 = 2,
    v2 = 4,
    v3 = 8,
    v4 = 16,
    v5 = 32,
    v6 = 64,
    v7 = 128,

    //edges
    e0 = v0 | v1,
    e1 = v1 | v2,
    e2 = v2 | v3,
    e3 = v3 | v0,
    e4 = v4 | v5,
    e5 = v5 | v6,
    e6 = v6 | v7,
    e7 = v7 | v4,
    e8 = v0 | v4,
    e9 = v1 | v5,
    e10 = v2 | v6,
    e11 = v3 | v7,

    //faces
    bottom = e0 | e1 | e2 | e3,
    top = e7 | e4 | e5 | e6,
    left = e8 | e3 | e11 | e7,
    right = e9 | e1 | e10 | e5,
    back = e0 | e9 | e4 | e8,
    front = e2 | e10 | e6 | e11,

    All = v0 | v1 | v2 | v3 | v4 | v5 | v6 | v7
}