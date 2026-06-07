
namespace UnityCommander.Common.Styling.Converters
{
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    /// Специализированный Thumb-элемент для изменения ширины столбцов GridView.
    /// Используется как "ползунок" между колонками, позволяя
    /// пользователю перетаскивать границу столбца, чтобы изменить его ширину.
    /// 
    /// Основная логика:
    /// - При нажатии ЛКМ курсор меняется на горизонтальный ресайзер (SizeWE)
    /// - При отпускании ЛКМ курсор возвращается в нормальное состояние
    /// </summary>
    internal class GridViewColumnThumb : Thumb
    {
        /// <summary>
        /// Конструктор — ничего дополнительного не делает,
        /// просто инициализирует базовый Thumb.
        /// </summary>
        public GridViewColumnThumb()
            : base()
        {
        }

        /// <summary>
        /// Вызывается при нажатии левой кнопки мыши.
        /// Устанавливает курсор в режим горизонтального изменения размера (SizeWE),
        /// чтобы визуально показать, что элемент можно тянуть влево/вправо.
        /// </summary>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.SizeWE; // курсор "↔"
            base.OnMouseLeftButtonDown(e);         // стандартная логика Thumb
        }

        /// <summary>
        /// Вызывается при отпускании левой кнопки мыши.
        /// Возвращает курсор мыши обратно в нормальное состояние.
        /// </summary>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e); // стандартная логика Thumb
            Mouse.OverrideCursor = null; // восстановить курсор по умолчанию
        }
    }
}
