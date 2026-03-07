using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Games_Store.Models;

namespace Games_Store.Views
{
    public class GameSelectedEventArgs : RoutedEventArgs
    {
        public Game Game { get; }
        public GameSelectedEventArgs(RoutedEvent routedEvent, Game game) : base(routedEvent)
        {
            Game = game;
        }
    }

    public partial class GameCarousel : UserControl
    {
        public static readonly RoutedEvent GameSelectedEvent =
            EventManager.RegisterRoutedEvent("GameSelected", RoutingStrategy.Bubble,
                typeof(EventHandler<GameSelectedEventArgs>), typeof(GameCarousel));

        public event EventHandler<GameSelectedEventArgs> GameSelected
        {
            add => AddHandler(GameSelectedEvent, value);
            remove => RemoveHandler(GameSelectedEvent, value);
        }

        // -- Configuration constants --
        private const double CardWidth = 280;
        private const double CardHeight = 360;
        private const double CenterScale = 1.0;
        private const double SideScale = 0.78;
        private const double FarScale = 0.58;
        private const double CenterOpacity = 1.0;
        private const double SideOpacity = 0.65;
        private const double FarOpacity = 0.30;
        private const double AnimationDurationMs = 450;
        private const double AutoScrollIntervalSec = 4.0;
        private const int VisibleSlots = 5; // far-left, left, center, right, far-right

        // -- State --
        private readonly List<Border> _cardElements = new();
        private readonly List<Rectangle> _dots = new();
        private readonly DispatcherTimer _autoScrollTimer;
        private ObservableCollection<Game> _games = new();
        private int _currentIndex;
        private bool _isAnimating;
        private bool _isPaused;

        // Dependency property for binding games from a ViewModel
        public static readonly DependencyProperty GamesSourceProperty =
            DependencyProperty.Register(
                nameof(GamesSource),
                typeof(ObservableCollection<Game>),
                typeof(GameCarousel),
                new PropertyMetadata(null, OnGamesSourceChanged));

        public ObservableCollection<Game> GamesSource
        {
            get => (ObservableCollection<Game>)GetValue(GamesSourceProperty);
            set => SetValue(GamesSourceProperty, value);
        }

        private static void OnGamesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GameCarousel carousel && e.NewValue is ObservableCollection<Game> games)
            {
                carousel._games = games;
                carousel._currentIndex = 0;
                carousel.BuildCards();
                carousel.LayoutCards(animate: false);
                carousel.UpdateIndicators();
            }
        }

        // -- Gradient palette for card covers --
        private static readonly string[][] GradientPalette =
        [
            ["#0f3460", "#e94560"],
            ["#1a1a2e", "#4834d4"],
            ["#2d132c", "#c72c41"],
            ["#0a1931", "#185adb"],
            ["#1b1b2f", "#e43f5a"],
            ["#1a1a40", "#4ecca3"],
        ];

        public GameCarousel()
        {
            InitializeComponent();

            _autoScrollTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(AutoScrollIntervalSec)
            };
            _autoScrollTimer.Tick += (_, _) => MoveNext();

            Loaded += OnLoaded;
            SizeChanged += (_, _) => LayoutCards(animate: false);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Grab focus so keyboard events work immediately
            Focus();

            if (_games.Count > 0 && _cardElements.Count == 0)
            {
                BuildCards();
                LayoutCards(animate: false);
                UpdateIndicators();
            }

            _autoScrollTimer.Start();
        }

        // =============================================
        //  Card creation
        // =============================================

        private void BuildCards()
        {
            CarouselCanvas.Children.Clear();
            _cardElements.Clear();

            for (int i = 0; i < _games.Count; i++)
            {
                var card = CreateCard(_games[i], i);
                // Initialize Canvas positions to avoid NaN animation issues
                Canvas.SetLeft(card, 0);
                Canvas.SetTop(card, 0);
                card.Opacity = 0;
                _cardElements.Add(card);
                CarouselCanvas.Children.Add(card);
            }
        }

        private Border CreateCard(Game game, int index)
        {
            // Pick a gradient from the palette
            var palette = GradientPalette[index % GradientPalette.Length];
            var gradientBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop((Color)ColorConverter.ConvertFromString(palette[0]), 0),
                    new GradientStop((Color)ColorConverter.ConvertFromString(palette[1]), 1),
                }
            };

            // Cover area: use image if URL exists, otherwise gradient + icon
            Border coverBorder;
            bool hasImage = !string.IsNullOrEmpty(game.ImageUrl);

            if (hasImage)
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(game.ImageUrl, UriKind.Absolute);
                    bitmap.DecodePixelWidth = (int)CardWidth;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                    bitmap.EndInit();

                    var grid = new Grid();
                    grid.Children.Add(new Border
                    {
                        Background = gradientBrush,
                        Child = new TextBlock
                        {
                            Text = "\uE7FC",
                            FontFamily = new FontFamily("Segoe MDL2 Assets"),
                            FontSize = 52,
                            Foreground = new SolidColorBrush(Color.FromArgb(120, 255, 255, 255)),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    });
                    grid.Children.Add(new Image
                    {
                        Source = bitmap,
                        Stretch = Stretch.UniformToFill,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    });

                    coverBorder = new Border
                    {
                        Height = 180,
                        Background = Brushes.Black,
                        CornerRadius = new CornerRadius(12, 12, 0, 0),
                        ClipToBounds = true,
                        Child = grid
                    };
                }
                catch
                {
                    coverBorder = new Border
                    {
                        Height = 180,
                        Background = gradientBrush,
                        CornerRadius = new CornerRadius(12, 12, 0, 0),
                        ClipToBounds = true,
                        Child = new TextBlock
                        {
                            Text = "\uE7FC",
                            FontFamily = new FontFamily("Segoe MDL2 Assets"),
                            FontSize = 52,
                            Foreground = new SolidColorBrush(Color.FromArgb(120, 255, 255, 255)),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    };
                }
            }
            else
            {
                coverBorder = new Border
                {
                    Height = 180,
                    Background = gradientBrush,
                    CornerRadius = new CornerRadius(12, 12, 0, 0),
                    ClipToBounds = true,
                    Child = new TextBlock
                    {
                        Text = "\uE7FC",
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        FontSize = 52,
                        Foreground = new SolidColorBrush(Color.FromArgb(120, 255, 255, 255)),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };
            }

            // Rating stars
            var starText = GenerateStarText(game.Rating);
            var ratingPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 6, 0, 0) };
            ratingPanel.Children.Add(new TextBlock
            {
                Text = starText,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffd700")!),
                FontSize = 13
            });
            ratingPanel.Children.Add(new TextBlock
            {
                Text = $"  {game.Rating:F1}",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aaa")!),
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            });

            // Info section
            var infoPanel = new StackPanel { Margin = new Thickness(14, 12, 14, 14) };
            infoPanel.Children.Add(new TextBlock
            {
                Text = game.Title,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                TextTrimming = TextTrimming.CharacterEllipsis
            });
            infoPanel.Children.Add(new TextBlock
            {
                Text = game.Genre,
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888")!),
                Margin = new Thickness(0, 2, 0, 4)
            });
            infoPanel.Children.Add(new TextBlock
            {
                Text = game.Description,
                FontSize = 11,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#bbb")!),
                TextWrapping = TextWrapping.Wrap,
                MaxHeight = 32,
                TextTrimming = TextTrimming.CharacterEllipsis
            });
            infoPanel.Children.Add(ratingPanel);

            var priceRow = new Grid { Margin = new Thickness(0, 8, 0, 0) };
            priceRow.Children.Add(new TextBlock
            {
                Text = $"${game.Price:F2}",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4ecca3")!),
                HorizontalAlignment = HorizontalAlignment.Left
            });
            infoPanel.Children.Add(priceRow);

            // Card container
            var cardContent = new StackPanel();
            cardContent.Children.Add(coverBorder);
            cardContent.Children.Add(infoPanel);

            var card = new Border
            {
                Width = CardWidth,
                Height = CardHeight,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#16213e")!),
                CornerRadius = new CornerRadius(12),
                ClipToBounds = true,
                Cursor = Cursors.Hand,
                Tag = game,
                Child = cardContent,
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 20,
                    ShadowDepth = 4,
                    Opacity = 0.6
                },
                // GPU-friendly: use RenderTransform for all animations
                RenderTransform = new TransformGroup
                {
                    Children = new TransformCollection
                    {
                        new ScaleTransform(1, 1),
                        new TranslateTransform(0, 0)
                    }
                },
                RenderTransformOrigin = new Point(0.5, 0.5),
                CacheMode = new BitmapCache()  // GPU caching
            };

            card.MouseLeftButtonUp += OnCardClick;

            return card;
        }

        private void OnCardClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is Game game && border.IsHitTestVisible)
            {
                RaiseEvent(new GameSelectedEventArgs(GameSelectedEvent, game));
            }
        }

        private static string GenerateStarText(double rating)
        {
            int full = (int)rating;
            bool half = (rating - full) >= 0.3;
            int empty = 5 - full - (half ? 1 : 0);
            return new string('\u2605', full) + (half ? "\u00BD" : "") + new string('\u2606', empty);
        }

        // =============================================
        //  Layout and animation
        // =============================================

        private void LayoutCards(bool animate)
        {
            if (_cardElements.Count == 0) return;

            double canvasWidth = CarouselCanvas.ActualWidth > 0 ? CarouselCanvas.ActualWidth : ActualWidth > 0 ? ActualWidth : 1000;
            double centerX = canvasWidth / 2.0;
            double centerY = (CarouselCanvas.Height - CardHeight) / 2.0;
            double spacing = CardWidth * 0.72;

            // Slot definitions relative to center: position offset, scale, opacity, z-index
            var slots = new (double offsetX, double scale, double opacity, int zIndex)[]
            {
                (-spacing * 2, FarScale,    FarOpacity,    1),  // far left
                (-spacing,     SideScale,   SideOpacity,   2),  // left
                (0,            CenterScale, CenterOpacity, 3),  // center
                (spacing,      SideScale,   SideOpacity,   2),  // right
                (spacing * 2,  FarScale,    FarOpacity,    1),  // far right
            };

            int count = _games.Count;
            var duration = TimeSpan.FromMilliseconds(AnimationDurationMs);
            var easing = new CubicEase { EasingMode = EasingMode.EaseInOut };

            for (int i = 0; i < _cardElements.Count; i++)
            {
                var card = _cardElements[i];

                // Calculate the relative position from current center index (with wrapping)
                int relativePos = GetWrappedOffset(i, _currentIndex, count);

                // Find which slot this maps to (center = slot index 2)
                int slotIndex = relativePos + 2; // relativePos: -2,-1,0,1,2 => slotIndex: 0,1,2,3,4

                bool isVisible = slotIndex >= 0 && slotIndex < VisibleSlots;

                if (!isVisible)
                {
                    // Stop any running position animations and set a valid position
                    card.BeginAnimation(Canvas.LeftProperty, null);
                    card.BeginAnimation(Canvas.TopProperty, null);
                    card.BeginAnimation(OpacityProperty, null);
                    Canvas.SetLeft(card, centerX - CardWidth / 2.0);
                    Canvas.SetTop(card, centerY);
                    card.Opacity = 0;
                    card.IsHitTestVisible = false;
                    Panel.SetZIndex(card, 0);
                    continue;
                }

                var slot = slots[slotIndex];
                double targetX = centerX + slot.offsetX - CardWidth / 2.0;
                double targetY = centerY;

                card.IsHitTestVisible = slotIndex == 2; // Only center card is interactive

                if (animate)
                {
                    AnimateCard(card, targetX, targetY, slot.scale, slot.opacity, slot.zIndex, duration, easing);
                }
                else
                {
                    SetCardPosition(card, targetX, targetY, slot.scale, slot.opacity, slot.zIndex);
                }

                // Highlight glow on center card
                if (card.Effect is DropShadowEffect shadow)
                {
                    var targetColor = slotIndex == 2
                        ? (Color)ColorConverter.ConvertFromString("#e94560")!
                        : Colors.Black;
                    double targetBlur = slotIndex == 2 ? 30 : 20;

                    if (animate)
                    {
                        shadow.BeginAnimation(DropShadowEffect.ColorProperty,
                            new ColorAnimation(targetColor, duration) { EasingFunction = easing });
                        shadow.BeginAnimation(DropShadowEffect.BlurRadiusProperty,
                            new DoubleAnimation(targetBlur, duration) { EasingFunction = easing });
                    }
                    else
                    {
                        shadow.Color = targetColor;
                        shadow.BlurRadius = targetBlur;
                    }
                }
            }
        }

        private static int GetWrappedOffset(int itemIndex, int centerIndex, int count)
        {
            if (count == 0) return 0;
            int diff = itemIndex - centerIndex;
            // Wrap around for infinite looping
            if (diff > count / 2) diff -= count;
            if (diff < -(count / 2)) diff += count;
            return diff;
        }

        private static void SetCardPosition(Border card, double x, double y, double scale, double opacity, int zIndex)
        {
            var tg = (TransformGroup)card.RenderTransform;
            var st = (ScaleTransform)tg.Children[0];
            var tt = (TranslateTransform)tg.Children[1];

            // TranslateTransform works relative to the element's original position
            // Since we're using RenderTransform, we set Canvas.Left/Top for base position
            // and use ScaleTransform for scaling
            Canvas.SetLeft(card, x);
            Canvas.SetTop(card, y);
            st.ScaleX = scale;
            st.ScaleY = scale;
            card.Opacity = opacity;
            Panel.SetZIndex(card, zIndex);
        }

        private static void AnimateCard(Border card, double x, double y, double scale, double opacity,
            int zIndex, TimeSpan duration, IEasingFunction easing)
        {
            Panel.SetZIndex(card, zIndex);

            var tg = (TransformGroup)card.RenderTransform;
            var st = (ScaleTransform)tg.Children[0];

            // Read current values safely (fallback to target if NaN)
            double fromX = Canvas.GetLeft(card);
            double fromY = Canvas.GetTop(card);
            if (double.IsNaN(fromX)) fromX = x;
            if (double.IsNaN(fromY)) fromY = y;

            // Animate position via Canvas.Left / Canvas.Top
            card.BeginAnimation(Canvas.LeftProperty,
                new DoubleAnimation(fromX, x, duration) { EasingFunction = easing });
            card.BeginAnimation(Canvas.TopProperty,
                new DoubleAnimation(fromY, y, duration) { EasingFunction = easing });

            // Animate scale
            st.BeginAnimation(ScaleTransform.ScaleXProperty,
                new DoubleAnimation(scale, duration) { EasingFunction = easing });
            st.BeginAnimation(ScaleTransform.ScaleYProperty,
                new DoubleAnimation(scale, duration) { EasingFunction = easing });

            // Animate opacity
            AnimateOpacity(card, opacity, duration, easing);
        }

        private static void AnimateOpacity(UIElement element, double target,
            TimeSpan duration, IEasingFunction easing)
        {
            element.BeginAnimation(OpacityProperty,
                new DoubleAnimation(target, duration) { EasingFunction = easing });
        }

        // =============================================
        //  Navigation
        // =============================================

        private void MoveNext()
        {
            if (_isAnimating || _games.Count < 2) return;
            _isAnimating = true;

            _currentIndex = (_currentIndex + 1) % _games.Count;
            LayoutCards(animate: true);
            UpdateIndicators();

            // Unlock after animation completes
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(AnimationDurationMs + 50) };
            timer.Tick += (_, _) => { _isAnimating = false; timer.Stop(); };
            timer.Start();
        }

        private void MovePrev()
        {
            if (_isAnimating || _games.Count < 2) return;
            _isAnimating = true;

            _currentIndex = (_currentIndex - 1 + _games.Count) % _games.Count;
            LayoutCards(animate: true);
            UpdateIndicators();

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(AnimationDurationMs + 50) };
            timer.Tick += (_, _) => { _isAnimating = false; timer.Stop(); };
            timer.Start();
        }

        // =============================================
        //  Indicators (dots)
        // =============================================

        private void UpdateIndicators()
        {
            IndicatorPanel.Children.Clear();
            _dots.Clear();

            for (int i = 0; i < _games.Count; i++)
            {
                var dot = new Rectangle
                {
                    Width = i == _currentIndex ? 20 : 8,
                    Height = 8,
                    RadiusX = 4,
                    RadiusY = 4,
                    Fill = i == _currentIndex
                        ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e94560")!)
                        : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444")!),
                    Margin = new Thickness(3, 0, 3, 0),
                    Cursor = Cursors.Hand
                };

                int capturedIndex = i;
                dot.MouseLeftButtonDown += (_, _) => JumpTo(capturedIndex);

                _dots.Add(dot);
                IndicatorPanel.Children.Add(dot);
            }
        }

        private void JumpTo(int index)
        {
            if (_isAnimating || index == _currentIndex) return;
            _isAnimating = true;

            _currentIndex = index;
            LayoutCards(animate: true);
            UpdateIndicators();

            ResetAutoScroll();

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(AnimationDurationMs + 50) };
            timer.Tick += (_, _) => { _isAnimating = false; timer.Stop(); };
            timer.Start();
        }

        // =============================================
        //  Event handlers
        // =============================================

        private void OnPrevClick(object sender, RoutedEventArgs e)
        {
            MovePrev();
            ResetAutoScroll();
        }

        private void OnNextClick(object sender, RoutedEventArgs e)
        {
            MoveNext();
            ResetAutoScroll();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                MovePrev();
                ResetAutoScroll();
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                MoveNext();
                ResetAutoScroll();
                e.Handled = true;
            }
        }

        private void OnMouseEnterCarousel(object sender, MouseEventArgs e)
        {
            _isPaused = true;
            _autoScrollTimer.Stop();
        }

        private void OnMouseLeaveCarousel(object sender, MouseEventArgs e)
        {
            _isPaused = false;
            _autoScrollTimer.Start();
        }

        private void ResetAutoScroll()
        {
            _autoScrollTimer.Stop();
            if (!_isPaused)
                _autoScrollTimer.Start();
        }
    }
}
