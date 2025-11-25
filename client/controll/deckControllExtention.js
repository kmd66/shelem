
import { Propertys as p } from './public';
export class DeckControllExtention {
    constructor() {
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

    createCarts(list, my) {
        let cards = [];
        list.forEach((u, index) => {
            if (u.count == 0) return;
            for (let i = 0; i < u.count; i++) {
                const id = this.generateId();
                const isLastInnerIteration = i === u.count - 1;
                let div;
                const elModel = isLastInnerIteration ? u : {};
                div = this.getEl(elModel, id);
                this.setGroundStyle(div, index, i, my);
                cards.push({
                    id: id,
                    el: div,
                    suit: elModel.suit,
                    rank: elModel.rank
                });
            }
        });
        return cards;
    }

    setGroundStyle(div, index, xM, my) {
        let y = p.h - p.remToPx(9.5);
        if (!my)
            y = p.remToPx(3.5);
        let x = p.remToPx(7);
        if (index > 1) x = x * -1;
        else if (index > 0) x = 0;
        x = p.wC - p.remToPx(2.5) + x - xM;
        div.style.top = y + 'px';
        div.style.left = x + 'px';
        //div.style.transform = 'scale(0.9)';
    }

    getEl(model, id) {
        const div = document.createElement('div');
        div.className = this.getClassName(model.suit, model.rank);
        div.id = `I${id}`;
        div.innerHTML = '<div class="back"></div>';
        if (model.suit != undefined && model.suit > -1) {
            div.innerHTML = '<div class="face"></div>';
        }
        return div;
    }

    getClassName(suit, rank) {
        if (suit == undefined || suit < 0)
            return 'card g';

        let c = 'card g';
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

