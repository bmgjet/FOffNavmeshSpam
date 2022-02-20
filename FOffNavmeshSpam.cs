using UnityEngine;
using UnityEngine.AI;

namespace Oxide.Plugins
{
    [Info("FOffNavmeshSpam", "bmgjet", "1.0.0")]
    [Description("Stops the navmesh spamming for bots")]
    public class FOffNavmeshSpam : RustPlugin
    {
        void OnEntitySpawned(BaseNetworkable entity)
        {
            BaseNavigator baseNavigator = entity.GetComponent<BaseNavigator>();
            if (baseNavigator != null)
            {
                Vector3 pos;
                if (!baseNavigator.GetNearestNavmeshPosition(entity.transform.position + (Vector3.one * 2f), out pos, (baseNavigator.IsSwimming() ? 30f : 6f)))
                {
                    baseNavigator.topologyPreference = (TerrainTopology.Enum)TerrainTopology.EVERYTHING;
                    BasePlayer bp = null;
                    bp = entity as BasePlayer;
                    if (bp != null){ClipGround(bp, baseNavigator);}
                }
            }
        }
        private void ClipGround(BasePlayer bp, BaseNavigator baseNavigator)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(bp.transform.position, out hit, 30, (int)baseNavigator.topologyPreference))
            {
                bp.gameObject.layer = 17;
                baseNavigator.Warp(hit.position);
                bp.SendNetworkUpdateImmediate();
            }
            NextTick(() =>
            {
                Vector3 pos;
                if (!baseNavigator.GetNearestNavmeshPosition(bp.transform.position + (Vector3.one * 2f), out pos, (baseNavigator.IsSwimming() ? 30f : 6f)))
                {
                    Puts("No Navmesh found @ " + pos.ToString() + " bot will be stationary to stop spam.");
                    baseNavigator.CanUseNavMesh = false;
                }
            });
        }
    }
}
