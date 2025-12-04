import { Propertys as p } from './public';
import { ElTop } from './elTop';
import { ElHokm } from './elHokm';
import { StartPage } from './startPage';

class Els {
    constructor() {
        this.top = new ElTop();
        this.hokm = new ElHokm();
        this.startPage = new StartPage();
    }

    static instance;
    static Instance() {
        if (!Els.instance) {
            Els.instance = new Els();
        }
        return Els.instance;
    }

}

if (!window.__els__) {
    window.__els__ = Els.Instance();
}
export const els = window.__els__;