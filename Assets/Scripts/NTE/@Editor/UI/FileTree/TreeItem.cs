using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using DG.Tweening;

using NTE.Core.UI;
using NTE.Core.Utils;
using NTE.Editor.UI.Controlling;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NTE.Editor.UI.FileTree
{
    public class TreeItem : RectBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public RawImage Icon;
        public TMP_Text Header;
        public TreeItem Parent;
        public FileTree Tree;
        public Transform Content;

        public FileSystemInfo Information;

        public ObservableDictionary<string, TreeItem> Items = new();
        private FileType m_Type;

        public float HoverAlpha
        {
            set
            {
                hoverAlpha = value;
                Background.DOFade(value + selAlpha, 0.1f);
            }
        }
        private float hoverAlpha;
        public float SelectAlpha
        {
            set
            {
                selAlpha = value;
                Background.DOFade(value + hoverAlpha, 0.1f);
            }
        }
        private float selAlpha;

        public string RelativePath
        {
            get
            {
                if (IsRoot)
                    return string.Empty;
                StringBuilder sb = new();
                sb.Append(Parent.RelativePath);
                sb.Append('/');
                sb.Append(Name);
                return sb.ToString();
            }
        }

        public bool IsRoot => Parent == null;

        public FileType Type { get => m_Type; set { IconTexture = value.GetIcon(); m_Type = value; } }

        public string FullPath
        {
            get => Information.FullName;
            set
            {
                if (Directory.Exists(value))
                    Information = new DirectoryInfo(value);
                else
                    Information = new FileInfo(value);
                Name = Information.Name;
            }
        }

        public Texture2D IconTexture { set => Icon.texture = value; }
        public string Name { get => Header.text; set => Header.text = value; }

        private RawImage Background => GetComponent<RawImage>();

        private void Awake()
        {
            Items = new();
            Items.OnItemChanged += OnItemChanged;
            OnItemChanged();
        }

        public void OnItemChanged(OnItemChangedEventArgs<string, TreeItem> e) => OnItemChanged();
        public void OnItemChanged()
        {
            float y = -24;
            foreach (KeyValuePair<string, TreeItem> i in Items)
            {
                i.Value.transform.DOAnchorPosY(y, 0.1f).SetEase(Ease.OutQuad);
                if (i.Value.transform.sizeDelta.y == 0)
                    i.Value.OnItemChanged();
                y -= i.Value.transform.sizeDelta.y;
            }
            y -= Header.rectTransform.sizeDelta.y;
            transform.sizeDelta = new(transform.sizeDelta.x, -y);
            if (Parent != null)
                Parent.OnItemChanged();
        }

        public void AddChild(string path)
        {
            TreeItem i = Instantiate(Resources.Load<GameObject>("Prefabs/Item"), Content).GetComponent<TreeItem>();
            i.Tree = Tree;
            i.Parent = this;
            i.Initialize(path);
            Items.Add(i.Name, i);
        }

        #region 文件系统
        public void Initialize(string path)
        {
            FullPath = path;
            SetType();
        }

        public void SetType()
        {
            string name = Path.GetFileName(FullPath).ToLower();
            string extension = Path.GetExtension(FullPath).ToLower();

            if (Directory.Exists(FullPath))
            {
                if (Parent == null)
                {
                    Type = FileType.Root;
                    return;
                }

                else if (Parent.Type == FileType.Root)
                {
                    Type = name switch
                    {
                        "configs" => FileType.ConfigsDirectory,
                        "resources" => FileType.ResourcesDirectory,
                        "scenarios" => FileType.ScenariosDirectory,
                        _ => FileType.UnknownDirectory
                    };
                    return;
                }

                if (Parent.Type == FileType.ScenariosDirectory)
                {
                    Type = FileType.ScenarioChapter;
                    return;
                }

                if (Parent.Type == FileType.ConfigsDirectory)
                {
                    Type = name switch
                    {
                        "cgs" => FileType.CGConfigs,
                        "characters" => FileType.CharacterConfigs,
                        _ => FileType.UnknownDirectory
                    };
                    return;
                }

                if (Parent.Type == FileType.ResourcesDirectory)
                {
                    Type = name switch
                    {
                        "sprites" => FileType.Sprites,
                        "vocals" => FileType.Vocals,
                        _ => FileType.UnknownDirectory
                    };
                    return;
                }

                if (Parent.Type == FileType.Sprites || Parent.Type == FileType.SpritesChildDirectory)
                {
                    Type = FileType.SpritesChildDirectory;
                    return;
                }
                if (Parent.Type == FileType.Vocals || Parent.Type == FileType.VocalsChildDirectory)
                {
                    Type = FileType.VocalsChildDirectory;
                    return;
                }
            }
            else
            {
                if (Parent == null)
                    throw new Exception("根目录不能是文件");
                if (Parent.Type == FileType.ScenarioChapter && extension.Equals(".scn"))
                {
                    Type = FileType.ScenarioScript;
                    return;
                }
                if (extension.Equals(".tcf"))
                {
                    Type = Parent.Type switch
                    {
                        FileType.CGConfigs => FileType.CGConfig,
                        FileType.CharacterConfigs => FileType.CharacterConfig,
                        _ => FileType.UnknownDirectory
                    };
                    return;
                }
                else if (extension.Equals(".png") && (Parent.Type == FileType.Sprites || Parent.Type == FileType.SpritesChildDirectory))
                {
                    Type = FileType.Sprite;
                    return;
                }
                else if (extension.Equals(".ogg") && (Parent.Type == FileType.Vocals || Parent.Type == FileType.VocalsChildDirectory))
                {
                    Type = FileType.Vocal;
                    return;
                }
            }
        }

        public void Delete()
        {
            foreach (TreeItem i in Items.Values)
                i.Delete();
            Information.Delete();
        }

        public void OnDeleted()
        {
            Parent.Items.Remove(Name);
            Destroy(gameObject);
        }
        #endregion

        #region GUI
        public void OnPointerUp(PointerEventData eventData)
        {
            Tree.SelectedIndex = RelativePath;
            if (eventData.button == PointerEventData.InputButton.Right)
                RightMenu.Create(Tree.FolderConfig, eventData.position);
        }

        public void OnPointerClick(PointerEventData eventData)
        { }

        public void OnPointerDown(PointerEventData eventData)
        { }

        public void Select()
        {
            SelectAlpha = 0.1f;
        }

        public void Deselect()
        {
            SelectAlpha = 0;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            HoverAlpha = 0.02f;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HoverAlpha = 0;
        }
        #endregion
    }
}
