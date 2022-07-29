using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Arlo.Entities
{
    public class GroundSpike : Attacker
    {
        public static GroundSpike fabricate(Entity target, float warningTime, int sortingOrder)
        {
            GameObject instance = new GameObject("spike");
            instance.transform.position = PixelSnapper.Snap(target.transform.position + 0.25f*Vector3.down);
            instance.AddComponent<SpriteRenderer>();
            GroundSpike script = instance.AddComponent<GroundSpike>();
            script.attackee = target;
            script.warningTime = warningTime;
            script.sr.sprite = GlobalStorage.spikeWarnings[0];
            script.sr.sortingOrder = sortingOrder;
            script.sr.spriteSortPoint = SpriteSortPoint.Pivot;
            script.hitPS = GameObject.Find("Hit Particle System").GetComponent<ParticleSystem>();

            return script;
        }

        /// <summary>
        /// The amount of time the ground spike warns the player for before coming out of the ground.
        /// </summary>
        public float warningTime;

        protected override void StackableAwake()
        {
            base.StackableAwake();
            MaxHealth = 100;
            Health = 100;
        }

        protected override void StackableUpdate()
        {
            base.StackableUpdate();
        }

        public IEnumerator attack()
        {
            while (Time.timeSinceLevelLoad + entityTimeOffset <= warningTime)
            {
                for (int i = 0; i < GlobalStorage.spikeWarnings.Length; i++)
                {
                    sr.sprite = GlobalStorage.spikeWarnings[i];
                    yield return new WaitForSeconds(0.45f*Mathf.Pow((Time.timeSinceLevelLoad + entityTimeOffset)/warningTime - 1,2) + 0.05f);
                }
            }

            // Create container for sprite and sprite mask.
            GameObject container = new GameObject("mask conatiner");
            container.AddComponent<SortingGroup>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
            container.transform.parent = transform;
            container.transform.localPosition = Vector3.up * 2f / 16f;

            // Create collider.
            GameObject collider = Instantiate(GlobalStorage.ellipsisCollider, transform);
            collider.transform.localPosition = Vector3.zero;
            collider.transform.localScale = Vector3.zero;
            collider.GetComponent<PolygonCollider2D>().isTrigger = true;

            // Add event trigger to collider.
            var eventTrigger = collider.AddComponent<EventTrigger>();
            eventTrigger.obj = attackee.gameObject;
            eventTrigger.onEnter.AddListener(() => attackee.Dead = true);
            
            // Create mask.
            GameObject maskObject = new GameObject("mask");
            maskObject.transform.parent = container.transform;
            maskObject.transform.localPosition = Vector3.zero;
            var mask = maskObject.AddComponent<SpriteMask>();
            mask.sprite = GlobalStorage.spikeMask;
            mask.spriteSortPoint = SpriteSortPoint.Pivot;

            // Create spike.
            GameObject instance = new GameObject("spike");
            instance.transform.parent = container.transform;
            instance.transform.localPosition = Vector3.down*3.625f;
            instance.AddComponent<SpriteRenderer>();
            var spikeEntity = instance.AddComponent<Entity>();
            spikeEntity.speed = 10;
            spikeEntity.sr.sprite = GlobalStorage.spike;
            spikeEntity.sr.spriteSortPoint = SpriteSortPoint.Pivot;
            spikeEntity.sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

            // Move spike up
            bool wait = true;
            float timeStarted = Time.timeSinceLevelLoad;
            spikeEntity.linearlyGo(Vector3.zero, () => wait = false);
            spikeEntity.onUpdate(() =>
            {
                var t = (Time.timeSinceLevelLoad - timeStarted) * spikeEntity.speed / 3.625f;
                collider.transform.localPosition = Vector3.up * 0.8125f * t;
                collider.transform.localScale = new Vector3(3.75f, 1.375f, 0) * t;
                if (t >= 1)
                {
                    collider.transform.localScale = new Vector3(3.75f, 1.375f, 0);
                    collider.transform.localPosition = Vector3.up * 0.8125f;
                    return true;
                }
                return false;
            });

            yield return new WaitUntil(() => wait);
            yield return new WaitForSeconds(1);

            // Destroy spike after a second once spike is out.
            Destroy(gameObject);
        }
    }
}
