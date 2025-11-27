import { Propertys as p } from './public';
import { DeckEvents } from './deckEvents';

export class DeckControllExtention {
    constructor() {
        this.events = new DeckEvents();
    }

    removeAllDesk(deck, isEl) {
        if (!deck) return;

        if (isEl) {
            deck.forEach(card => {
                card.el.remove();
            });
            deck = null;
        }
        else {
            deck.cards.forEach(card => {
                card.unmount();
                card.$el.remove();
            });
            deck.cards = [];
            deck = null;
        }
    }
    removeAllCards(deck, cards, isEl) {

        if (isEl) {
            cards.forEach(card => {
                card.el.remove();
            });
            deck = deck.filter(deckCard => !cards.includes(deckCard));
        }
        else {
            cards.forEach(card => {
                if (card.$el && card.$el.parentNode) {
                    card.$el.parentNode.removeChild(card.$el);
                }
            });
            deck.cards = deck.cards.filter(deckCard =>
                !cards.includes(deckCard)
            );
        }

    }

    createFanCarts(list, my) {
        this.elClassName = 'f';
        let cards = [];
        let y = p.h + p.remToPx(5);
        if (!my)
            y = -p.remToPx(4);
        const x = p.wC - p.remToPx(2.5) ;
        list.forEach((u, i) => {
            const id = this.generateId();
            const elModel = my ? u : {};
            let div = this.getEl(elModel, id);
            div.style.top = y + 'px';
            div.style.left = x + 'px';
            div.style.zIndex = 10;
            cards.push({
                id: id,
                el: div,
                suit: elModel.suit,
                rank: elModel.rank,
                loc: { x: x, y: y },
                my: my,
                type: 'Fan'
            });

            this.events.init(div, my);
        });

        const length = cards.length > 1 ? cards.length : 2;
        const dz = length > 12 ? 6 : length > 8 ? 8 : length > 5 ? 10 : cards.length > 1 ? 14 : 1;
        const distance = 110; // افزایش فاصله
        const df = length * dz;
        var df2 = df / 2.5;

        cards = sortCards(cards);
        cards.forEach((card, index) => {
            const rot = index / (length - 1) * df - df2;
            if (my)
                card.el.style.transform = `
        translate(${Math.cos(deg2rad(rot - 90)) * distance}px, ${Math.sin(deg2rad(rot - 90)) * distance}px) 
        rotate(${rot}deg)`;
            else
                card.el.style.transform = `
        translate(${Math.cos(deg2rad(rot + 90)) * distance}px, ${Math.sin(deg2rad(rot + 90)) * distance}px) 
        rotate(${rot}deg)`;
        });
        return cards;



        function deg2rad(degrees) {
            return degrees * Math.PI / 180;
        }
        function sortCards(cards) {
            return cards.sort((a, b) => {
                if (a.suit !== b.suit) {
                    return String(a.suit).localeCompare(String(b.suit));
                }
                return a.rank - b.rank;
            });
        }
    }

    createGroundCarts(list, my) {
        this.elClassName = 'g';
        let cards = [];
        list.forEach((u, index) => {
            if (u.count == 0) return;
            for (let i = 0; i < u.count; i++) {
                const id = this.generateId();
                const isLastInnerIteration = i === u.count - 1;
                let div;
                const elModel = isLastInnerIteration ? u : {};
                div = this.getEl(elModel, id);
                const loc = this.setGroundStyle(index, i, my);
                div.style.top = loc.y + 'px';
                div.style.left = loc.x + 'px';
                cards.push({
                    id: id,
                    el: div,
                    suit: elModel.suit,
                    rank: elModel.rank,
                    loc: loc,
                    my: my,
                    type:'Ground'
                });

                this.events.init(div, my);
            }
        });
        return cards;
    }

    setGroundStyle(index, xM, my) {
        let y = p.h - p.remToPx(5.5);
        if (!my)
            y = p.remToPx(7.5);
        let x = p.remToPx(7);
        if (index > 1) x = x * -1;
        else if (index > 0) x = 0;
        x = p.wC - p.remToPx(2.14) + x - xM;
        return { x: x, y: y };
    }

    getEl(model, id) {
        const div = document.createElement('div');
        div.className = this.getClassName(model.suit, model.rank);
        div.id = `${id}`;
        div.innerHTML = '<div class="back"></div>';
        if (model.suit != undefined && model.suit > -1) {
            div.innerHTML = '<div class="face"></div>';
        }
        return div;
    }

    getClassName(suit, rank) {
        let c = 'card ' + this.elClassName;
        if (suit == undefined || suit < 0)
            return c;

        switch (suit) {
            case 0: c += ' spades'; break;
            case 1: c += ' hearts'; break;
            case 2: c += ' clubs'; break;
            case 3: c += ' diamonds'; break;
        }
        c += ` rank${rank}`;
        return c;
    }

    generateId() {
        return Date.now().toString(36) + Math.random().toString(36).substring(2);
    }
}

