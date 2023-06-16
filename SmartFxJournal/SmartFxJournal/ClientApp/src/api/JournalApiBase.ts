import { RestApi } from './ApiBase';

export abstract class JournalApi<T> extends RestApi {

    protected readonly resourceName:string

    constructor(resource : string) {
        super();
        this.resourceName = (resource.startsWith('/')) ? resource : '/' + resource;
    };

    protected abstract getKeyOf(input : T) : string; 

    public async getAll(params?: Map<string, string>) : Promise<T[]> {
        let path = this.resourceName + super.getQueryString(params);
        return super.index<T>(path);
    }

    public async get(key: string, params?: Map<string, string>) : Promise<T> {
        let path = this.resourceName + '/' + key + super.getQueryString(params);
        return super.single<T>(path);
    }

    public async createNew(input : T) : Promise<T> {
        return super.post<T>(this.resourceName, input);
    }

    public async update(input : T) : Promise<T> {
        return super.put(this.resourceName + '/' + this.getKeyOf(input) , input);
    }

    public async delete(input : string | T) : Promise<boolean> {
        let resKey : string;
        if (typeof input === 'string') {
            resKey = input;
        } else {
            resKey = this.getKeyOf(input);
        }
        return super.delete(this.resourceName + '/' + resKey);
    }
}