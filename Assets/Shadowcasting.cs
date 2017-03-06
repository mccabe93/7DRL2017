﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadowcasting : MonoBehaviour
{

    DungeonGenerator dg = new DungeonGenerator();

    public int visualRange = 5;
    private List<Vector2> visibleTiles = new List<Vector2>();

    List<int> activeOctants = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };

    public Vector2 caster { get; set; }

	// Use this for initialization
	void Start () {
        dg.init();
        caster = new Vector2(3, 0);
        getVisibleCells();
	}
	
    public void getVisibleCells()
    {
        visibleTiles = new List<Vector2>();
        foreach(int oct in activeOctants)
        {
            shadowcastOctant(1, oct, 1.0f, 0.0f);
        }
    }

    public void shadowcastOctant(int depth, int octant, float startSlope, float endSlope)
    {
        int visualRangeSquared = visualRange * visualRange;
        int x = 0,
            y = 0;

        switch (octant)
        {
            case 1:
                y = (int)(caster.y - depth);
                if (y < 0) return;

                x = (int)caster.x - System.Convert.ToInt32((startSlope * System.Convert.ToDouble(depth)));
                if (x < 0) x = 0;

                while (getSlope(x, y, caster.x, caster.y, false) >= endSlope)
                {
                    if (getVisibleDistance(new Vector2(x, y)) <= visualRangeSquared)
                    {
                        if (dg.map[x, y] == 1)
                        {
                            if (x - 1 >= 0 && dg.map[x - 1, y] == 0)
                            {
                                shadowcastOctant(depth + 1, octant, startSlope, getSlope(x - 0.5f, y + 0.5f, caster.x, caster.y, false));
                            }
                            else
                            {
                                if (x - 1 >= 0 && dg.map[x - 1, y] == 1)
                                {
                                    startSlope = getSlope(x - 0.5f, y - 0.5f, caster.x, caster.y, false);
                                }
                                visibleTiles.Add(new Vector2(x, y));
                            }
                        }
                    }
                    x++;
                }
                x--;
                break;
            case 2:
                y = (int)(caster.y - depth);
                if (y < 0) return;

                x = (int)caster.x + System.Convert.ToInt32((startSlope * System.Convert.ToDouble(depth)));
                if (x >= ApplicationConstants.DUNGEON_WIDTH) x = ApplicationConstants.DUNGEON_WIDTH - 1;

                while (getSlope(x, y, caster.x, caster.y, false) <= endSlope)
                {
                    if (getVisibleDistance(new Vector2(x, y)) <= visualRangeSquared)
                    {
                        if (dg.map[x, y] == 1)
                        {
                            if (x + 1 < ApplicationConstants.DUNGEON_WIDTH && dg.map[x + 1, y] == 0)
                            {
                                shadowcastOctant(depth + 1, octant, startSlope, getSlope(x - 0.5f, y + 0.5f, caster.x, caster.y, false));
                            }
                            else
                            {
                                if (x + 1 < ApplicationConstants.DUNGEON_WIDTH && dg.map[x + 1, y] == 1)
                                {
                                    startSlope = getSlope(x + 0.5f, y + 0.5f, caster.x, caster.y, false);
                                }
                                visibleTiles.Add(new Vector2(x, y));
                            }
                        }
                    }
                    x--;
                }
                x++;
                break;
            case 3:
                x = (int)(caster.x + depth);
                if (x >= ApplicationConstants.DUNGEON_WIDTH) return;

                y = (int)caster.y - System.Convert.ToInt32((startSlope * System.Convert.ToDouble(depth)));
                if (y < 0) y = 0;

                while (getSlope(x, y, caster.x, caster.y, false) <= endSlope)
                {
                    if (getVisibleDistance(new Vector2(x, y)) <= visualRangeSquared)
                    {
                        if (dg.map[x, y] == 1)
                        {
                            if (y - 1 >= 0 && dg.map[x, y - 1] == 0)
                            {
                                shadowcastOctant(depth + 1, octant, startSlope, getSlope(x - 0.5f, y + 0.5f, caster.x, caster.y, false));
                            }
                            else
                            {
                                if (y - 1 >= 0 && dg.map[x, y - 1] == 1)
                                {
                                    startSlope = getSlope(x + 0.5f, y - 0.5f, caster.x, caster.y, true);
                                }
                                visibleTiles.Add(new Vector2(x, y));
                            }
                        }
                    }
                    y++;
                }
                y--;
                break;
            case 4:
                x = (int)(caster.x + depth);
                if (x >= ApplicationConstants.DUNGEON_WIDTH) return;

                y = (int)caster.y + System.Convert.ToInt32((startSlope * System.Convert.ToDouble(depth)));
                if (y >= ApplicationConstants.DUNGEON_HEIGHT) y = ApplicationConstants.DUNGEON_HEIGHT - 1;

                while (getSlope(x, y, caster.x, caster.y, true) >= endSlope)
                {
                    if (getVisibleDistance(new Vector2(x, y)) <= visualRangeSquared)
                    {
                        if (dg.map[x, y] == 1)
                        {
                            if (y + 1 < ApplicationConstants.DUNGEON_HEIGHT && dg.map[x, y + 1] == 0)
                            {
                                shadowcastOctant(depth + 1, octant, startSlope, getSlope(x - 0.5f, y + 0.5f, caster.x, caster.y, false));
                            }
                            else
                            {
                                if (y + 1 < ApplicationConstants.DUNGEON_HEIGHT && dg.map[x, y + 1] == 1)
                                {
                                    startSlope = getSlope(x + 0.5f, y + 0.5f, caster.x, caster.y, true);
                                }
                                visibleTiles.Add(new Vector2(x, y));
                            }
                        }
                    }
                    y--;
                }
                y++;
                break;
        }

        if (x < 0)
            x = 0;
        else if (x >= ApplicationConstants.DUNGEON_WIDTH)
            x = ApplicationConstants.DUNGEON_WIDTH - 1;

        if (y < 0)
            y = 0;
        else if (y >= ApplicationConstants.DUNGEON_HEIGHT)
            y = ApplicationConstants.DUNGEON_HEIGHT - 1;

        if (depth < visualRange && dg.map[x, y] == 0)
            shadowcastOctant(depth+1, octant, startSlope, endSlope);
    }

    private float getSlope(float x0, float y0, float x1, float y1, bool invert)
    {
        if (invert)
            return (y0 - y1) / (x0 - x1);

        return (x0 - x1) / (y0 - y1); 
    }

    private int getVisibleDistance(Vector2 position)
    {
        return (int)((position.x - caster.x) * (position.x - caster.x) + (position.y - caster.y) * (position.y - caster.y));
    }
}
