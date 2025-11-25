

// ماژول Shuffle
class ModernDeckShuffle {
    static deck(deck) {
        deck.shuffle = deck.queued(ModernDeckShuffle.shuffle.bind(deck));
    }

    static shuffle(next) {
        const cards = this.cards;
        const fontSize = ModernDeckShuffle.getFontSize();

        ModernDeckShuffle.fisherYates(cards);

        let completed = 0;
        cards.forEach((card, i) => {
            card.pos = i;
            card.shuffle((i) => {
                completed++;
                if (completed === cards.length) {
                    next();
                }
            });
        });
    }

    static card(card) {
        card.shuffle = function (cb) {
            const i = card.pos;
            const z = i / 4;
            const delay = i * 2;

            card.animateTo({
                delay,
                duration: 200,
                x: ModernDeckShuffle.plusminus(Math.random() * 40 + 20) * ModernDeckShuffle.getFontSize() / 16,
                y: -z,
                rot: 0
            });

            card.animateTo({
                delay: 200 + delay,
                duration: 200,
                x: -z,
                y: -z,
                rot: 0,
                onStart: () => {
                    card.$el.style.zIndex = i;
                },
                onComplete: () => {
                    cb(i);
                }
            });
        };
    }

    static fisherYates(array) {
        for (let i = array.length - 1; i; i--) {
            const rnd = Math.floor(Math.random() * i);
            [array[i], array[rnd]] = [array[rnd], array[i]];
        }
        return array;
    }

    static plusminus(value) {
        return Math.random() > 0.5 ? value : -value;
    }

    static getFontSize() {
        return parseFloat(getComputedStyle(document.body).fontSize);
    }
}

// ماژول Sort
class ModernDeckSort {
    static deck(deck) {
        deck.sort = deck.queued(ModernDeckSort.sort.bind(deck));
    }

    static sort(next, reverse) {
        const cards = this.cards;

        cards.sort((a, b) => {
            return reverse ? a.i - b.i : b.i - a.i;
        });

        cards.forEach((card, i) => {
            card.sort(i, cards.length, (i) => {
                if (i === cards.length - 1) {
                    next();
                }
            }, reverse);
        });
    }

    static card(card) {
        card.sort = function (i, len, cb, reverse) {
            const z = i / 4;
            const delay = i * 10;

            card.animateTo({
                delay,
                duration: 400,
                x: -z,
                y: -150,
                rot: 0,
                onComplete: () => {
                    card.$el.style.zIndex = i;
                }
            });

            card.animateTo({
                delay: delay + 500,
                duration: 400,
                x: -z,
                y: -z,
                rot: 0,
                onComplete: () => {
                    cb(i);
                }
            });
        };
    }
}

// ماژول Fan
class ModernDeckFan {
    static deck(deck) {
        deck.fan = deck.queued(ModernDeckFan.fan.bind(deck));
    }

    static fan(next) {
        const cards = this.cards;
        const len = cards.length;
        const fontSize = ModernDeckFan.getFontSize();

        cards.forEach((card, i) => {
            card.fan(i, len, (i) => {
                if (i === cards.length - 1) {
                    next();
                }
            });
        });
    }

    static card(card) {
        card.fan = function (i, len, cb) {
            const z = i / 4;
            const delay = i * 10;
            const df = len * 7;
            const df2 = df / 3;
            const rot = i / (len - 1) * df - df2;

            card.animateTo({
                delay,
                duration: 300,
                x: -z,
                y: -z,
                rot: 0
            });

            card.animateTo({
                delay: 300 + delay,
                duration: 300,
                x: Math.cos(ModernDeckFan.deg2rad(rot - 90)) * 55 * ModernDeckFan.getFontSize() / 16,
                y: Math.sin(ModernDeckFan.deg2rad(rot - 90)) * 55 * ModernDeckFan.getFontSize() / 16,
                rot: rot,
                onStart: () => {
                    card.$el.style.zIndex = i;
                },
                onComplete: () => {
                    cb(i);
                }
            });
        };
    }

    static deg2rad(degrees) {
        return degrees * Math.PI / 180;
    }

    static getFontSize() {
        return parseFloat(getComputedStyle(document.body).fontSize);
    }
}

// ماژول Flip
class ModernDeckFlip {
    static deck(deck) {
        deck.flip = deck.queued(ModernDeckFlip.flip.bind(deck));
    }

    static flip(next, side) {
        const cards = this.cards;
        const flipped = cards.filter(card => card.side === 'front').length / cards.length;

        cards.forEach((card, i) => {
            card.setSide(side ? side : flipped > 0.5 ? 'back' : 'front');
        });
        next();
    }
}

// ماژول Poker
class ModernDeckPoker {
    static deck(deck) {
        deck.poker = deck.queued(ModernDeckPoker.poker.bind(deck));
    }

    static poker(next) {
        const cards = this.cards;
        const len = cards.length;
        const fontSize = ModernDeckPoker.getFontSize();

        cards.slice(-5).reverse().forEach((card, i) => {
            card.poker(i, len, (i) => {
                card.setSide('front');
                if (i === 4) {
                    next();
                }
            });
        });
    }

    static card(card) {
        card.poker = function (i, len, cb) {
            const delay = i * 250;

            card.animateTo({
                delay,
                duration: 250,
                x: Math.round((i - 2.05) * 70 * ModernDeckPoker.getFontSize() / 16),
                y: Math.round(-110 * ModernDeckPoker.getFontSize() / 16),
                rot: 0,
                onStart: () => {
                    card.$el.style.zIndex = len - 1 + i;
                },
                onComplete: () => {
                    cb(i);
                }
            });
        };
    }

    static getFontSize() {
        return parseFloat(getComputedStyle(document.body).fontSize);
    }
}

// ماژول Intro
class ModernDeckIntro {
    static deck(deck) {
        deck.intro = deck.queued(ModernDeckIntro.intro.bind(deck));
    }

    static intro(next) {
        const cards = this.cards;

        cards.forEach((card, i) => {
            card.setSide('front');
            card.intro(i, (i) => {
                this.animationFrames(250, 0).start(() => {
                    card.setSide('back');
                });
                if (i === cards.length - 1) {
                    next();
                }
            });
        });
    }

    static card(card) {
        card.intro = function (i, cb) {
            const delay = 500 + i * 10;
            const z = i / 4;
            const transform = card.deck.prefix('transform');

            card.$el.style[transform] = card.deck.translate(`${-z}px`, '-250px');
            card.$el.style.opacity = 0;

            card.x = -z;
            card.y = -250 - z;
            card.rot = 0;

            card.animateTo({
                delay,
                duration: 1000,
                x: -z,
                y: -z,
                onStart: () => {
                    card.$el.style.zIndex = i;
                },
                onProgress: (t) => {
                    card.$el.style.opacity = t;
                },
                onComplete: () => {
                    card.$el.style.opacity = '';
                    cb?.(i);
                }
            });
        };
    }
}

// ماژول BySuit
class ModernDeckBysuit {
    static deck(deck) {
        deck.bysuit = deck.queued(ModernDeckBysuit.bysuit.bind(deck));
    }

    static bysuit(next) {
        const cards = this.cards;
        const fontSize = ModernDeckBysuit.getFontSize();

        cards.forEach((card, i) => {
            card.bysuit((i) => {
                if (i === cards.length - 1) {
                    next();
                }
            });
        });
    }

    static card(card) {
        card.bysuit = function (cb) {
            const i = card.i;
            const delay = i * 10;
            const rank = card.rank;
            const suit = card.suit;

            card.animateTo({
                delay,
                duration: 400,
                x: -Math.round((6.75 - rank) * 8 * ModernDeckBysuit.getFontSize() / 16),
                y: -Math.round((1.5 - suit) * 92 * ModernDeckBysuit.getFontSize() / 16),
                rot: 0,
                onComplete: () => {
                    cb(i);
                }
            });
        };
    }

    static getFontSize() {
        return parseFloat(getComputedStyle(document.body).fontSize);
    }
}

// حالا Card Class رو کامل کنم:
class ModernCard {
    constructor(deck, index) {
        this.deck = deck;
        this.i = index;
        this.rank = index % 13 + 1;
        this.suit = Math.floor(index / 13);
        this.z = (52 - index) / 4;
        this.x = -this.z;
        this.y = -this.z;
        this.rot = 0;
        this.side = 'back';
        this.pos = index;

        this.$el = deck.createElement('div');
        this.$face = deck.createElement('div');
        this.$back = deck.createElement('div');

        this.isDraggable = false;
        this.isFlippable = false;

        this.initElement();
        this.loadModules();
        this.addEventListeners();
    }

    initElement() {
        const transform = this.deck.prefix('transform');

        this.$face.classList.add('face');
        this.$back.classList.add('back');

        this.$el.style[transform] = this.deck.translate(`${-this.z}px`, `${-this.z}px`);
        this.setSide('back');
        this.setRankSuit(this.rank, this.suit);
    }

    loadModules() {
        // بارگذاری ماژول‌ها برای کارت
        ModernDeckShuffle.card(this);
        ModernDeckSort.card(this);
        ModernDeckFan.card(this);
        ModernDeckPoker.card(this);
        ModernDeckIntro.card(this);
        ModernDeckBysuit.card(this);
    }

    addEventListeners() {
        const onMousedown = (e) => this.handleMouseDown(e);
        this.$el.addEventListener('mousedown', onMousedown);
        this.$el.addEventListener('touchstart', onMousedown);
    }

    // بقیه متدهای کارت...
    mount(target) {
        this.$root = target;
        this.$root.appendChild(this.$el);
    }

    unmount() {
        if (this.$root) {
            this.$root.removeChild(this.$el);
            this.$root = null;
        }
    }

    setSide(newSide) {
        if (newSide === 'front') {
            if (this.side === 'back') {
                this.$el.removeChild(this.$back);
            }
            this.side = 'front';
            this.$el.appendChild(this.$face);
            this.setRankSuit(this.rank, this.suit);
        } else {
            if (this.side === 'front') {
                this.$el.removeChild(this.$face);
            }
            this.side = 'back';
            this.$el.appendChild(this.$back);
            this.$el.setAttribute('class', 'card');
        }
    }

    setRankSuit(rank, suit) {
        const suitName = this.suitName(suit);
        this.$el.setAttribute('class', `card ${suitName} rank${rank}`);
    }

    suitName(suit) {
        return suit === 0 ? 'spades' : suit === 1 ? 'hearts' :
            suit === 2 ? 'clubs' : suit === 3 ? 'diamonds' : 'joker';
    }

    animateTo(params) {
        const {
            delay,
            duration,
            x = this.x,
            y = this.y,
            rot = this.rot,
            ease = 'cubicInOut',
            onStart,
            onProgress,
            onComplete
        } = params;

        let startX, startY, startRot;

        this.deck.animationFrames(delay, duration)
            .start(() => {
                startX = this.x || 0;
                startY = this.y || 0;
                startRot = this.rot || 0;
                onStart?.();
            })
            .progress((t) => {
                const et = ModernDeck.ease[ease](t);
                const diffX = x - startX;
                const diffY = y - startY;
                const diffRot = rot - startRot;

                onProgress?.(t, et);

                this.x = startX + diffX * et;
                this.y = startY + diffY * et;
                this.rot = startRot + diffRot * et;

                const transform = this.deck.prefix('transform');
                this.$el.style[transform] =
                    this.deck.translate(`${this.x}px`, `${this.y}px`) +
                    (diffRot ? ` rotate(${this.rot}deg)` : '');
            })
            .end(() => {
                onComplete?.();
            });
    }

    handleMouseDown(e) {
        // پیاده‌سازی drag & drop...
        e.preventDefault();

        if (!this.isDraggable) return;

        const startPos = { x: e.clientX || e.touches[0].clientX, y: e.clientY || e.touches[0].clientY };
        const startX = this.x;
        const startY = this.y;

        const onMousemove = (e) => {
            if (!this.isDraggable) return;
            const currentX = e.clientX || e.touches[0].clientX;
            const currentY = e.clientY || e.touches[0].clientY;

            this.x = startX + (currentX - startPos.x);
            this.y = startY + (currentY - startPos.y);

            const transform = this.deck.prefix('transform');
            this.$el.style[transform] = this.deck.translate(`${this.x}px`, `${this.y}px`);
        };

        const onMouseup = () => {
            window.removeEventListener('mousemove', onMousemove);
            window.removeEventListener('mouseup', onMouseup);
            window.removeEventListener('touchmove', onMousemove);
            window.removeEventListener('touchend', onMouseup);
        };

        window.addEventListener('mousemove', onMousemove);
        window.addEventListener('mouseup', onMouseup);
        window.addEventListener('touchmove', onMousemove);
        window.addEventListener('touchend', onMouseup);
    }

    enableDragging() {
        if (this.isDraggable) return;
        this.isDraggable = true;
        this.$el.style.cursor = 'move';
    }

    disableDragging() {
        if (!this.isDraggable) return;
        this.isDraggable = false;
        this.$el.style.cursor = '';
    }

    enableFlipping() {
        if (this.isFlippable) return;
        this.isFlippable = true;
    }

    disableFlipping() {
        if (!this.isFlippable) return;
        this.isFlippable = false;
    }
}

export class ModernDeck3 {
    constructor(jokers = false) {
        this.ticking = false;
        this.animations = [];
        this.modules = ModernDeck.modules;
        this.cards = new Array(jokers ? 55 : 52);
        this.$el = this.createElement('div');
        this.$el.classList.add('deck');

        this.initQueue();
        this.initObservable();
        this.loadModules();
        this.createCards();
    }

    static modules = {
        bysuit: ModernDeckBysuit,
        fan: ModernDeckFan,
        intro: ModernDeckIntro,
        poker: ModernDeckPoker,
        shuffle: ModernDeckShuffle,
        sort: ModernDeckSort,
        flip: ModernDeckFlip
    };
    createElement(type) {
        return document.createElement(type);
    }
    // بقیه متدها همون قبلی...
}


export class ModernDeck {
    constructor(jokers = false) {
        this.ticking = false;
        this.animations = [];
        this.modules = ModernDeck.modules;
        this.cards = new Array(jokers ? 55 : 52);
        this.$el = this.createElement('div');
        this.$el.classList.add('deck');

        this.initQueue();
        this.initObservable();
        this.loadModules();
        this.createCards();
    }

    static modules = {
        bysuit: ModernDeckBysuit,
        fan: ModernDeckFan,
        intro: ModernDeckIntro,
        poker: ModernDeckPoker,
        shuffle: ModernDeckShuffle,
        sort: ModernDeckSort,
        flip: ModernDeckFlip
    };

    static ease = {
        linear: t => t,
        quadIn: t => t * t,
        quadOut: t => t * (2 - t),
        quadInOut: t => t < 0.5 ? 2 * t * t : -1 + (4 - 2 * t) * t,
        cubicIn: t => t * t * t,
        cubicOut: t => --t * t * t + 1,
        cubicInOut: t => t < 0.5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1,
        quartIn: t => t * t * t * t,
        quartOut: t => 1 - --t * t * t * t,
        quartInOut: t => t < 0.5 ? 8 * t * t * t * t : 1 - 8 * --t * t * t * t,
        quintIn: t => t * t * t * t * t,
        quintOut: t => 1 + --t * t * t * t * t,
        quintInOut: t => t < 0.5 ? 16 * t * t * t * t * t : 1 + 16 * --t * t * t * t * t
    };

    // Animation System
    animationFrames(delay, duration) {
        const now = Date.now();
        const start = now + delay;
        const end = start + duration;

        const animation = { start, end };
        this.animations.push(animation);

        if (!this.ticking) {
            this.ticking = true;
            requestAnimationFrame(() => this.tick());
        }

        return {
            start: (cb) => {
                animation.startcb = cb;
                return this;
            },
            progress: (cb) => {
                animation.progresscb = cb;
                return this;
            },
            end: (cb) => {
                animation.endcb = cb;
                return this;
            }
        };
    }

    tick() {
        const now = Date.now();

        if (!this.animations.length) {
            this.ticking = false;
            return;
        }

        for (let i = 0; i < this.animations.length; i++) {
            const animation = this.animations[i];

            if (now < animation.start) continue;

            if (!animation.started) {
                animation.started = true;
                animation.startcb?.();
            }

            const t = (now - animation.start) / (animation.end - animation.start);
            animation.progresscb?.(t < 1 ? t : 1);

            if (now > animation.end) {
                animation.endcb?.();
                this.animations.splice(i--, 1);
            }
        }

        requestAnimationFrame(() => this.tick());
    }

    // DOM Utilities
    createElement(type) {
        return document.createElement(type);
    }

    prefix(param) {
        if (!this.memoized) this.memoized = {};
        if (typeof this.memoized[param] !== 'undefined') {
            return this.memoized[param];
        }

        const style = document.createElement('p').style;
        if (typeof style[param] !== 'undefined') {
            this.memoized[param] = param;
            return param;
        }

        const camelCase = param[0].toUpperCase() + param.slice(1);
        const prefixes = ['webkit', 'moz', 'Moz', 'ms', 'o'];

        for (let prefix of prefixes) {
            const test = prefix + camelCase;
            if (typeof style[test] !== 'undefined') {
                this.memoized[param] = test;
                return test;
            }
        }

        return param;
    }

    translate(a, b, c = 0) {
        if (typeof this.has3d === 'undefined') {
            this.has3d = this.check3d();
        }

        if (this.has3d) {
            return `translate3d(${a}, ${b}, ${c})`;
        } else {
            return `translate(${a}, ${b})`;
        }
    }

    check3d() {
        const isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
        if (!isMobile) return false;

        const transform = this.prefix('transform');
        const $p = this.createElement('p');
        document.body.appendChild($p);
        $p.style[transform] = 'translate3d(1px,1px,1px)';

        const has3d = $p.style[transform] && $p.style[transform] !== 'none';
        document.body.removeChild($p);

        return has3d;
    }

    // Card Creation
    createCards() {
        for (let i = this.cards.length; i; i--) {
            this.cards[i - 1] = new ModernCard(this, i - 1);
            this.cards[i - 1].setSide('back');
            this.cards[i - 1].mount(this.$el);
        }
    }

    // Queue System
    initQueue() {
        this.queueing = [];

        this.queue = (action) => {
            if (!action) return;

            this.queueing.push(action);
            if (this.queueing.length === 1) {
                this.next();
            }
        };

        this.queued = (action) => {
            return (...args) => {
                this.queue((next) => {
                    action.apply(this, [next, ...args]);
                });
            };
        };

        this.next = () => {
            this.queueing[0]((err) => {
                if (err) throw err;
                this.queueing = this.queueing.slice(1);
                if (this.queueing.length) this.next();
            });
        };
    }

    // Observable System
    initObservable() {
        this.listeners = {};

        this.on = (name, cb, ctx) => {
            if (!this.listeners[name]) this.listeners[name] = [];
            this.listeners[name].push({ cb, ctx });
        };

        this.one = (name, cb, ctx) => {
            if (!this.listeners[name]) this.listeners[name] = [];
            this.listeners[name].push({ cb, ctx, once: true });
        };

        this.off = (name, cb) => {
            if (!name) {
                this.listeners = {};
                return;
            }
            if (!cb) {
                this.listeners[name] = [];
                return;
            }
            this.listeners[name] = this.listeners[name].filter(listener => listener.cb !== cb);
        };

        this.trigger = (name, ...args) => {
            const currentListeners = this.listeners[name] || [];
            this.listeners[name] = currentListeners.filter(listener => {
                listener.cb.apply(this, args);
                return !listener.once;
            });
        };
    }

    // Module System
    loadModules() {
        for (const moduleName in this.modules) {
            const module = this.modules[moduleName];
            module?.deck?.(this);
        }
    }

    // Public Methods
    mount(root) {
        this.$root = root;
        this.$root.appendChild(this.$el);
    }

    unmount() {
        if (this.$root) {
            this.$root.removeChild(this.$el);
            this.$root = null;
        }
    }

    // Deck Actions (will be populated by modules)
    shuffle() { /* Implemented by shuffle module */ }
    sort() { /* Implemented by sort module */ }
    fan() { /* Implemented by fan module */ }
    flip() { /* Implemented by flip module */ }
    poker() { /* Implemented by poker module */ }
    intro() { /* Implemented by intro module */ }
    bysuit() { /* Implemented by bysuit module */ }
}