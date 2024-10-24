using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Safari.MapComponents.Tiles
{
    [CreateAssetMenu(menuName = "Safari/Tile/Rule Tile")]
    public class SafariRuleTile : RuleTile, ISafariTile
    {
        [SerializeField] private bool isDescriptive;
        [SerializeField] private bool isStandardTile;
        [SerializeField] private List<string> themes;
        [SerializeField] private List<string> group;
        [SerializeField] private List<string> connectTo;
        private HashSet<string> hashSetGroup;
        private HashSet<string> connectToGroup;

        public bool GetDescriptive(Vector3Int position, Tilemap tilemap)
        {
            return isDescriptive;
        }

        public bool IsObservable => isDescriptive;
        public bool IsStandardTile => isStandardTile;
        public List<string> Themes { get => themes; set => themes = value; }
        public List<string> GroupData { get => group; set => group = value; }
        public HashSet<string> Group => hashSetGroup ??= group.ToHashSet();
        private HashSet<string> ConnectToGroup { get => connectToGroup ??= new HashSet<string>(connectTo); }
        public Sprite sprite => m_DefaultSprite;


        public TileRect GetRect(Vector3Int position, Tilemap tilemap)
        {
            return TileUtility.GetTileRect(m_DefaultSprite);
        }


        private void OnValidate()
        {
            //hashSetGroup = group.ToHashSet();
            //EditorUtility.SetDirty(this);
            //if (type != BrickType.notABrick && !group.Contains(type.ToString().ToTitleCase()))
            //    group.Add(type.ToString().ToTitleCase());
        }


        public override bool RuleMatch(int neighbor, TileBase other)
        {
            if (other is RuleOverrideTile)
                other = (other as RuleOverrideTile).m_InstanceTile;

            switch (neighbor)
            {
                case TilingRuleOutput.Neighbor.This:
                    {
                        if (other == this) return true;
                        if (other is ISafariTile libraryRuleTile && connectTo != null && libraryRuleTile.Group != null)
                        {
                            return RuleMatch(libraryRuleTile);
                        }
                        return false;
                    }
                case TilingRuleOutput.Neighbor.NotThis:
                    {
                        if (other == this) return false;
                        if (other is not ISafariTile libraryRuleTile || connectTo == null || libraryRuleTile.Group == null)
                        {
                            return true;
                        }
                        return !RuleMatch(libraryRuleTile);
                    }
                default:
                    {
                        if (other is ISafariTile libraryRuleTile)
                        {
                            if (connectTo != null && libraryRuleTile.Group != null)
                            {
                                return RuleMatch(libraryRuleTile);
                            }
                            if (ConnectToGroup.Contains("ANY"))
                            {
                                return RuleMatch(libraryRuleTile);
                            }
                        }
                    }
                    break;
            }

            return base.RuleMatch(neighbor, other);
        }

        private bool RuleMatch(ISafariTile libraryRuleTile)
        {
            return libraryRuleTile.Group.Any(t => ConnectToGroup.Contains(t));
        }

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
            TileUtility.SetGameObjectPosition(position, tilemap, go);
            return true;
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
        }

        [ContextMenu("Update Connect Group")]
        public void UpdateConnectGroup()
        {
            connectToGroup = new HashSet<string>(connectTo);
        }

    }
}