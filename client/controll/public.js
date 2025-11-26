class PropertyService {
    constructor() {
        this.startDeck = null;
        this.myFan = null;
        this.myGround = null;
        this.rivaFan = null;
        this.rivaGround = null;
    }
    static instance;
    static Instance() {
        if (!PropertyService.instance) {
            PropertyService.instance = new PropertyService();
        }
        return PropertyService.instance;
    }

    remToPx(x) {
        return x * parseFloat(getComputedStyle(document.documentElement).fontSize);
    };
    async init() {

        let params = new URLSearchParams(document.location.search);
        this.keys = {
            roomId: params.get("roomId"),
            key: params.get("userKey"),
            userId: params.get("userId")
        }
        const container = document.getElementById('container');
        this.w = container.offsetWidth;
        this.wC = container.offsetWidth / 2;
        this.h = container.offsetHeight;
        this.hC = container.offsetHeight / 2;
    }

    getCardById(id) {
        this.isDragging = false;

        const target = this.myFan.find(x => x.id == id);
        if (target) return target;
        return this.myGround.find(x => x.id == id);
    }
}

if (!window.__Propertys__) {
    window.__Propertys__ = PropertyService.Instance();
}
export const Propertys = window.__Propertys__;
