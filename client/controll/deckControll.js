import { Deck } from './deck';
import { gsap } from "gsap";
import { DeckControllExtention } from "./deckControllExtention";
import { Propertys as p } from './public';
class DeckControll {
    constructor() {
        this.container = document.getElementById('container');
        this.startDeck = null;
        this.myFan = null;
        this.myGround= null;
        this.rivaFan = null;
        this.rivaGround = null;
        this.extention = new DeckControllExtention();
    }

    static instance;
    static Instance() {
        if (!DeckControll.instance) {
            DeckControll.instance = new DeckControll();
        }
        return DeckControll.instance;
    }

    async start() {
        this.extention.removeAllDesk(this.startDeck);
        this.extention.removeAllDesk(this.myFan);
        this.extention.removeAllDesk(this.rivaFan);
        this.extention.removeAllDesk(this.myGround, true);
        this.extention.removeAllDesk(this.rivaGround, true);

        const container = document.getElementById('container');
        container.innerHTML = '';
        this.startDeck = Deck();
        await shuffle(this.startDeck);

        const tl = gsap.timeline();
        const tl2 = gsap.timeline();

        const cardsToSpread1 = this.startDeck.cards.slice(0, 24);
        const cardsToSpread2 = this.startDeck.cards.slice(24, 48);

        cardsToSpread1.forEach((card, index) => {
            const y = index % 2 === 0 ? p.hC : p.hC*-1;
            tl.to(card.$el, {
                x: 0 + index* -0.2,
                y: y,
                duration: 0.5,
            }, index * 0.05);
        });
        await tl.then();

        const yN = p.hC - p.remToPx(7);
        const xN = p.remToPx(7);
        cardsToSpread2.forEach((card, index) => {
            const y = index % 2 === 0 ? yN : yN * -1;
            let x = xN;
            if (index > 15) x = xN * -1;
            else if (index > 7) x = 0;
            tl2.to(card.$el, {
                x: x + index * -0.2,
                y: y,
                duration: 0.5,
            }, index * 0.05);
        });
        await tl2.then();

        this.extention.removeAllCards(this.startDeck, cardsToSpread1);
        this.extention.removeAllCards(this.startDeck, cardsToSpread2);
        async function shuffle(d) {
            d.mount(container);
            await new Promise(resolve => setTimeout(resolve, 700));
            d.shuffle();
            await new Promise(resolve => setTimeout(resolve, 700));
            d.shuffle();
            await new Promise(resolve => setTimeout(resolve, 700));
            d.shuffle();
            await new Promise(resolve => setTimeout(resolve, 700));
        }
    }

    async startFan(list) {
        this.extention.removeAllDesk(this.myFan);
        this.extention.removeAllDesk(this.rivaFan);
        const container = document.getElementById('container');

        this.rivaFan = new Deck();
        const removeCards = this.rivaFan.cards.slice(list.length, 52);
        this.extention.removeAllCards(this.rivaFan, removeCards);

        this.myFan = new Deck();
        const removeCards2 = this.myFan.cards.slice(list.length, 52);
        this.extention.removeAllCards(this.myFan, removeCards2);

        this.myFan.cards.forEach((card, index) => {
            card.suit = list[index].suit;
            card.rank = list[index].rank;
        });

        this.myFan.mount(document.getElementById('containerMyFan'));
        this.myFan.flip();
        this.myFan.fan();

        this.rivaFan.mount(document.getElementById('containerRivaFan'));
        this.rivaFan.fan();
        await new Promise(resolve => setTimeout(resolve, 5000));
    }

    async startGround(list) {
        this.extention.removeAllDesk(this.myGround, true);
        this.extention.removeAllDesk(this.rivaGround, true);

        const container = document.getElementById('container');

        this.myGround = this.extention.createCarts(list.u1, true);
        this.rivaGround = this.extention.createCarts(list.u2, false);

        this.myGround.forEach((c) => container.appendChild(c.el));
        this.rivaGround.forEach((c) => container.appendChild(c.el));

    }
    
}

if (!window.__deckControll__) {
    window.__deckControll__ = DeckControll.Instance();
}
export const deckControll = window.__deckControll__;
