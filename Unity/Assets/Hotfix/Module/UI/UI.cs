﻿using System.Collections.Generic;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class UiAwakeSystem: AwakeSystem<UI, string, GameObject>
    {
        public override void Awake(UI self, string name, GameObject gameObject)
        {
            self.Awake(name, gameObject);
        }
    }

    /// <summary>
    /// UI实体类
    /// </summary>
    [HideInHierarchy]
    public sealed class UI: Entity
    {
        public string Name { get; private set; }

        public Dictionary<string, UI> children = new Dictionary<string, UI>();

        /// <summary>
        /// UI窗体主控组件
        /// </summary>
        public UIBaseComponent UiComponent { get; private set; }

        /// <summary>
        /// 添加主UI组件，继承自UIBaseComponent
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <returns></returns>
        public K AddUiComponent<K>() where K : UIBaseComponent, new()
        {
            UiComponent = this.AddComponent<K>();
            return (K) UiComponent;
        }

        public K AddUiComponent<K, P1>(P1 p1) where K : UIBaseComponent, new()
        {
            UiComponent = this.AddComponent<K, P1>(p1);
            return (K) UiComponent;
        }

        public K AddUiComponent<K, P1, P2>(P1 p1, P2 p2) where K : UIBaseComponent, new()
        {
            UiComponent = this.AddComponent<K, P1, P2>(p1, p2);
            return (K) UiComponent;
        }

        public K AddUiComponent<K, P1, P2, P3>(P1 p1, P2 p2, P3 p3) where K : UIBaseComponent, new()
        {
            UiComponent = this.AddComponent<K, P1, P2, P3>(p1, p2, p3);
            return (K) UiComponent;
        }

        public void Awake(string name, GameObject gameObject)
        {
            this.children.Clear();
            gameObject.AddComponent<ComponentView>().Component = this;
            gameObject.layer = LayerMask.NameToLayer(LayerNames.UI);
            this.Name = name;
            this.GameObject = gameObject;
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            foreach (UI ui in this.children.Values)
            {
                ui.Dispose();
            }

            UnityEngine.Object.Destroy(GameObject);
            children.Clear();
        }

        public void SetAsFirstSibling()
        {
            this.GameObject.transform.SetAsFirstSibling();
        }

        public void Add(UI ui)
        {
            this.children.Add(ui.Name, ui);
            ui.Parent = this;
        }

        public void Remove(string name)
        {
            UI ui;
            if (!this.children.TryGetValue(name, out ui))
            {
                return;
            }

            this.children.Remove(name);
            ui.Dispose();
        }

        public UI Get(string name)
        {
            UI child;
            if (this.children.TryGetValue(name, out child))
            {
                return child;
            }

            GameObject childGameObject = this.GameObject.transform.Find(name)?.gameObject;
            if (childGameObject == null)
            {
                return null;
            }

            child = ComponentFactory.Create<UI, string, GameObject>(name, childGameObject);
            this.Add(child);
            return child;
        }
    }
}