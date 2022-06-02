using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Container //La caja de bombones
{
    CPResult ERROR_ZERO_MINUS = new CPResult(false, -1, "Hubo un error, mi cantidad temporal no puede ser menor a cero");
    CPResult CORRECT_PROCESS = new CPResult(true, 0, "el proceso se ejecutó correctamente");

    [SerializeField] float myTotalWeight;
    [SerializeField] List<Slot> slots;

    // el int de la Tuple es la calidad
    //Dictionary<Tuple<ElementData, int>, List<Slot>> element_register = new Dictionary<Tuple<ElementData, int>, List<Slot>>();

    //la key se mantiene, pero ahora vamos a usar una coleccion de int para los index que referencian a los slots
    Dictionary<Tuple<ElementData, int>, HashSet<int>> register = new Dictionary<Tuple<ElementData, int>, HashSet<int>>();

    public int Capacity => slots.Count;
    public Slot GetSlotByIndex(int index) => slots[index];

    #region CONSTRUCTOR
    public Container(int capacity)
    {
        slots = new List<Slot>();
        for (int i = 0; i < capacity; i++)
        {
            Slot slot = new Slot(i);
            slots.Add(slot);
        }
    }
    #endregion

    public CPResult Remove_Element(ElementData _data, int _quantity, int _quality, bool respect_quality = false)
    {
        var aux_temp = _quantity;
        //var tuple_key = Tuple.Create(_data, _quality);

        for(int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].IsEmpty)
            {
                if (respect_quality)
                {
                    if (slots[i].HasSameCompleteObject(_data, _quality))
                    {
                        #region Removido generico

                        var quant_in_slot = slots[i].Stack.Quantity;

                        if (aux_temp <= quant_in_slot)
                        {
                            //le remuevo mi auxiliar indiscriminadamente porque mi auxiliar es mas chico, o igual
                            //vacío totalmente mi auxiliar
                            if (!slots[i].RemoveElement(aux_temp)) throw new System.Exception("Hay algo mal en la cuenta");
                            aux_temp = 0;
                        }
                        else
                        {
                            //le quito lo que necesito a mi auxiliar y remuevo esa cantidad
                            aux_temp -= quant_in_slot;
                            if (!slots[i].RemoveElement(quant_in_slot)) throw new System.Exception("Hay algo mal en la cuenta");
                        }

                        #endregion
                    }
                    else continue;
                }
                else
                {
                    if (slots[i].HasSameOnlyElement(_data))
                    {
                        #region Removido generico

                        var quant_in_slot = slots[i].Stack.Quantity;

                        if (aux_temp <= quant_in_slot)
                        {
                            //le remuevo mi auxiliar indiscriminadamente porque mi auxiliar es mas chico, o igual
                            //vacío totalmente mi auxiliar
                            if (!slots[i].RemoveElement(aux_temp)) throw new System.Exception("Hay algo mal en la cuenta");
                            aux_temp = 0;
                        }
                        else
                        {
                            //le quito lo que necesito a mi auxiliar y remuevo esa cantidad
                            aux_temp -= quant_in_slot;
                            if (!slots[i].RemoveElement(quant_in_slot)) throw new System.Exception("Hay algo mal en la cuenta");
                        }

                        #endregion
                    }
                    else continue;
                }
            }
            else
            {
                continue;
            }

            if (aux_temp == 0) return CORRECT_PROCESS;
            if (aux_temp < 0) return ERROR_ZERO_MINUS;
            if (aux_temp > 0) continue;
        }


        return new CPResult(false, aux_temp, "ERROR: pudo llegar hasta acá porque no hay un checkeo previo, al remover no deberia tener un resto");
    }

    public CPResult Add_Element(ElementData l_data, int l_quantity, int l_quality)
    {
        var aux_temp = l_quantity;
        //var tuple_key = Tuple.Create(l_data, l_quality);

        //esto sirve para decirle al CPResult, che, laburé con estos casilleros
        List<int> used_positions_temp_registry = new List<int>();

        //por ahora evito la parte del registro rapido
        #region TO-DO: CAMBIAR POR EL REGISTRO DE HASHET<INT>

        ////primero me fijo si hay algun registro
        ////si lo hay me fijo que casilleros de ese registro, el primero tiene espacio
        //if (!element_register.ContainsKey(tuple_key))
        //{
        //    element_register.Add(tuple_key, new List<Slot>());
        //}
        //else
        //{
        //    var collection_to_check = element_register[tuple_key];
        //    foreach (var slot in collection_to_check)
        //    {

        //        var free = slot.Stack.FreeSpaces;
        //        Debug.Log("Espacio vacio es: " + free);

        //        if (aux_temp <= free)
        //        {
        //            Debug.Log("El valor es menor o igual, agrego todo y corto");
        //            //hago la suma directamente y corto la ejecucion porque ya agregaria todo
        //            slot.AddElement(aux_temp);
        //            aux_temp = 0;
        //        }
        //        else
        //        {
        //            Debug.Log("El valor es mayor, le quito lo que nesecito al aux y relleno el resto");
        //            aux_temp -= free;
        //            slot.AddElement(free);

        //            Debug.Log("me sobró: " + aux_temp);
        //        }

        //        if (aux_temp <= 0) break;
        //        else continue; //continuo revisando si hay otro para seguir restando
        //    }
        //}

        //if (aux_temp == 0) return new Container_Process_Result(true, aux_temp, "Proceso correcto: elementos agregador por registro");
        //if (aux_temp < 0) return ERROR_ZERO_MINUS;

        #endregion
        //si los registros estan todos llenos...

        //busco un casillero vacio
        foreach (var slot in slots)
        {
            //if (element_register[tuple_key].Contains(slot)) continue;

            if (slot.IsEmpty) // Encontré uno que esta vacio y no tiene los mismos datos
            {
                slot.CreateNewStack(l_data);
                slot.Stack.Quality = l_quality;

                #region Agregado generico
                var free_space_quant = slot.Stack.FreeSpaces;

                used_positions_temp_registry.Add(slot.Position);

                if (aux_temp <= free_space_quant) //hago la suma directamente y corto la ejecucion porque ya agregaria todo
                {
                    if (!slot.AddElement(aux_temp)) throw new System.Exception("Hay algo mal en la cuenta");
                    aux_temp = 0;
                }
                else // El valor es mayor, le quito lo que necesito al aux y relleno el resto
                {
                    aux_temp -= free_space_quant;
                    if (!slot.AddElement(free_space_quant)) throw new System.Exception("Hay algo mal en la cuenta");
                }
                #endregion

                Debug.Log("me sobró: " + aux_temp);

                #region TO-DO: Agregar al registro
                //y lo agrego al registro para la proxima pasada
                //var reg_slots = element_register[tuple_key];
                //reg_slots.Add(slot);
                #endregion

            }
            else // como no esta vacio tengo que checkear si es lo mismo que quiero agregar
            {
                if (slot.HasSameCompleteObject(l_data, l_quality))//el elemento que encontré es de mi mismo tipo, con la misma calidad
                {
                    #region Agregado generico
                    var free_space_quant = slot.Stack.FreeSpaces;

                    if (aux_temp <= free_space_quant) //hago la suma directamente y corto la ejecucion porque ya agregaria todo
                    {
                        if (!slot.AddElement(aux_temp)) throw new System.Exception("Hay algo mal en la cuenta");
                        aux_temp = 0;
                    }
                    else // El valor es mayor, le quito lo que necesito al aux y relleno el resto
                    {
                        aux_temp -= free_space_quant;
                        if (!slot.AddElement(free_space_quant)) throw new System.Exception("Hay algo mal en la cuenta");
                    }
                    #endregion
                    used_positions_temp_registry.Add(slot.Position);
                }
                else
                {
                    continue; //este slot tiene algo ya, y no es de mi tipo
                }
            }

            if (aux_temp == 0)
            {
                var message = CORRECT_PROCESS;
                message.AddSlotsEquiped(used_positions_temp_registry);
                return message;
            }
            if (aux_temp < 0) return ERROR_ZERO_MINUS;
            if (aux_temp > 0) continue;
        }

        return new CPResult(false, aux_temp, "INVENTARIO LLENO: Me sobran elementos", used_positions_temp_registry);
    }

    public bool QueryElement(ElementData l_data, int l_quantity, int l_quality, bool l_respect_quality = false)
    {
        var aux_temp = l_quantity;

        #region TO-DO: CAMBIAR POR EL REGISTRO DE HASHET<INT>
        //var tuple_key = Tuple.Create(l_data, l_quality);
        //preguntar primero al registro
        #endregion

        //busco un casillero vacio
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].IsEmpty)
            {
                if (l_respect_quality)
                {
                    if (slots[i].HasSameCompleteObject(l_data, l_quality))
                    {
                        //////////////////////
                        var quant_in_slot = slots[i].Stack.Quantity;

                        if (aux_temp <= quant_in_slot)
                        {
                            //vacío totalmente mi auxiliar porque es mas chico que lo que hay
                            aux_temp = 0;
                        }
                        else
                        {
                            //le quito lo que necesito a mi auxiliar y remuevo esa cantidad
                            aux_temp -= quant_in_slot;
                        }
                    }
                }
                else
                {
                    if (slots[i].HasSameOnlyElement(l_data))
                    {
                        //////////////////////
                        var quant_in_slot = slots[i].Stack.Quantity;

                        if (aux_temp <= quant_in_slot)
                        {
                            //vacío totalmente mi auxiliar porque es mas chico que lo que hay
                            aux_temp = 0;
                        }
                        else
                        {
                            //le quito lo que necesito a mi auxiliar y remuevo esa cantidad
                            aux_temp -= quant_in_slot;
                        }
                    }
                }
            }
            else
            {
                continue;
            }
        }

        #region Checkeo final
        if (aux_temp == 0) return true;
        else
        {
            if (aux_temp > 0) { /*me faltan*/ return false; }
            else { throw new System.Exception("Hubo un error en el calculo"); }
        }
        #endregion
    }


    void AddToSlot(ref Slot slot, ref int aux_temp)
    {
       
    }

    //esto es para lo nuevo... remplazamos el diccionario de lista de slots por hashet de indexs
    public List<Slot> GetListByIndexes(params int[] indexes)
    {
        return null;
    }

    #region DEPRECATED

    //public bool CheckIHaveElement(ElementData data, int quantity)
    //{
    //    var max_count = data.MaxQuality;
    //    var aux_temp = quantity;

    //    for (int i = 0; i < max_count; i++)
    //    {
    //        var quality = i + 1;
    //        var tuple_key = Tuple.Create(data, quality);

    //        if (element_register.ContainsKey(tuple_key))
    //        {
    //            var registered_slots = element_register[tuple_key];

    //            foreach (var slot in registered_slots)
    //            {
    //                if (aux_temp > 0)
    //                {
    //                    if (aux_temp >= slot.Quantity)
    //                    {
    //                        var to_remove = slot.Quantity;
    //                        aux_temp -= to_remove;
    //                    }
    //                    else
    //                    {
    //                        aux_temp = 0;
    //                    }
    //                }
    //                else
    //                {
    //                    break;
    //                }
    //            }

    //            if (aux_temp < 0) throw new System.Exception("el resto no deberia ser negativo");
    //            else if (aux_temp > 0) return false;
    //            else return true;
    //        }
    //        else
    //        {
    //            continue;
    //        }
    //    }
    //}

    //public bool CheckQuantityByQuality(ElementData data, int quantity, int quality)
    //{
    //    var aux_temp = quantity;
    //    var tuple_key = Tuple.Create(data, quality);

    //    if (element_register.ContainsKey(tuple_key))
    //    {
    //        var registered_slots = element_register[tuple_key];

    //        foreach (var slot in registered_slots)
    //        {
    //            if (aux_temp > 0)
    //            {
    //                if (aux_temp >= slot.Quantity)
    //                {
    //                    var to_remove = slot.Quantity;
    //                    aux_temp -= to_remove;
    //                }
    //                else
    //                {
    //                    aux_temp = 0;
    //                }
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }

    //        if (aux_temp < 0) throw new System.Exception("el resto no deberia ser negativo");
    //        else if (aux_temp > 0) return false;
    //        else return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    public CPResult RemoveElement(ElementData data, int quantity, int quality)
    {
        //var aux_temp = quantity;
        //var tuple_key = Tuple.Create(data, quality);

        //if (element_register.ContainsKey(tuple_key))
        //{
        //    var registered_slots = element_register[tuple_key];

        //    foreach(var slot in registered_slots)
        //    {
        //        if (aux_temp > 0)
        //        {
        //            if (aux_temp >= slot.Quantity)
        //            {
        //                var to_remove = slot.Quantity;
        //                slot.Empty();
        //                aux_temp -= to_remove;
        //            }
        //            else
        //            {
        //                slot.RemoveElement(aux_temp);
        //                aux_temp = 0;
        //            }
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }

        //    if (aux_temp < 0) return new Container_Process_Result(false, 0, "ERROR: el resto no deberia ser menor a cero");
        //    else if(aux_temp > 0) return new Container_Process_Result(false, 0, "ERROR: lo tengo en los registros, pero me sobraron unidades");
        //    else return new Container_Process_Result(true, 0, "CORRECTO: todo se quitó correctamente");
        //}
        //else
        //{
        //    return new Container_Process_Result(false, 0, "ERROR: estoy intentando quitar elementos de un registro inexistente");
        //}


        return new CPResult(true, 0, "Debug");
    }
    public CPResult AddElement(ElementData data, int quantity, int quality)
    {
        //me creo una variable de cantidad de elementos a agregar
        //porque tal vez vamos a pasar por el registro y luego por el buscador de casillas vacias
        var aux_temp = quantity;

        // Debug.Log("Intento agregar:: " + aux_temp + " " + data.Element_Name);

        //var tuple_key = Tuple.Create(data, quality);

        ////primero me fijo si hay algun registro
        ////si lo hay me fijo que casilleros de ese registro, el primero tiene espacio
        //if (!element_register.ContainsKey(tuple_key))
        //{
        //    element_register.Add(tuple_key, new List<Slot>());
        //}
        //else
        //{
        //    var collection_to_check = element_register[tuple_key];
        //    foreach (var slot in collection_to_check)
        //    {

        //        var free = slot.Stack.FreeSpaces;
        //        Debug.Log("Espacio vacio es: " + free);

        //        if (aux_temp <= free)
        //        {
        //            Debug.Log("El valor es menor o igual, agrego todo y corto");
        //            //hago la suma directamente y corto la ejecucion porque ya agregaria todo
        //            slot.AddElement(aux_temp);
        //            aux_temp = 0;
        //        }
        //        else
        //        {
        //            Debug.Log("El valor es mayor, le quito lo que nesecito al aux y relleno el resto");
        //            aux_temp -= free;
        //            slot.AddElement(free);

        //            Debug.Log("me sobró: " + aux_temp);
        //        }

        //        if (aux_temp <= 0) break;
        //        else continue; //continuo revisando si hay otro para seguir restando
        //    }
        //}

        //if (aux_temp == 0) return new Container_Process_Result(true, aux_temp, "Proceso correcto: elementos agregador por registro");
        //if (aux_temp < 0) return ERROR_ZERO_MINUS;

        //si mi aux_temp es mayor a cero... todavia tengo elementos para agregar

        //si los registros estan todos llenos...

        Debug.Log("Buscando un casillero vacio en Slots");

        //busco un casillero vacio
        foreach (var slot in slots)
        {
            //if (element_register[tuple_key].Contains(slot)) continue;

            if (slot.IsEmpty)
            {
                Debug.Log("Encontré uno que esta vacio y no tiene los mismos datos");

                slot.CreateNewStack(data);
                //slot.Stack.Quality = tuple_key.Item2;
                slot.Stack.Quality = quality;

                var free = slot.Stack.FreeSpaces;
                Debug.Log("Espacio vacio es: " + free);

                if (aux_temp <= free)
                {
                    Debug.Log("El valor es menor o igual, agrego todo y corto");
                    //hago la suma directamente y corto la ejecucion porque ya agregaria todo
                    slot.AddElement(aux_temp);
                    aux_temp = 0;
                }
                else
                {
                    Debug.Log("El valor es mayor, le quito lo que nesecito al aux y relleno el resto");
                    aux_temp -= free;
                    slot.AddElement(free);

                    Debug.Log("me sobró: " + aux_temp);
                }

                //y lo agrego al registro para la proxima pasada
                //var reg_slots = element_register[tuple_key];
                //reg_slots.Add(slot);

                if (aux_temp == 0) return new CPResult(true, aux_temp, "Proceso correcto: finalizo la busqueda de casilleros vacios");
                if (aux_temp < 0) return ERROR_ZERO_MINUS;
            }
            else
            {
                if (slot.HasSameCompleteObject(data, quality))
                {
                    return new CPResult(false, aux_temp, "ERROR: no deberia haber uno igual porque sino deberia estar en el diccionario");
                }
                else
                {
                    Debug.Log("Esto puede ser cuando agregue otros de otro tipo");
                }
            }
        }

        if (aux_temp == 0) return new CPResult(true, aux_temp, "Proceso correcto: aunque no deberia llegar acá");
        if (aux_temp < 0) return ERROR_ZERO_MINUS;


        return new CPResult(false, aux_temp, "INVENTARIO LLENO: Me sobran elementos");
    }
    #endregion

}

[System.Serializable]
public class Slot //Los separadores de bombones, [ESTÁTICO]
{
    [SerializeField] int position;
    public int Position { get { return position; } }
    [SerializeField] StackedPile stack;
    public StackedPile Stack => stack;

    public ElementData Element => stack.Element ? stack.Element : null;

    public int Quantity => stack.Quantity;

    public bool IsEmpty => stack.IsEmpty;
    public void CreateNewStack(ElementData element)
    {
        stack = new StackedPile();
        stack.SetElement(element);
    }

    public bool AddElement(int quantity)
    {
        return stack.Add_SAFE(quantity);
    }

    public bool RemoveElement(int quantity)
    {
        return stack.Remove_SAFE(quantity);
    }

    public bool HasSameData(Tuple<ElementData, int> tuple_key)
    {
        return stack.Full_Element_Is_Equal(tuple_key.Item1, tuple_key.Item2);
    }

    public bool HasSameCompleteObject(ElementData elem, int quality)
    {
        return stack.Full_Element_Is_Equal(elem, quality);
    }

    public bool HasSameOnlyElement(ElementData elem)
    {
        return stack.Only_Element_Is_Equal(elem);
    }



    #region Contructor
    public Slot(int position)
    {
        this.position = position;
        stack = new StackedPile();
    }
    #endregion

    #region Drag & Drop Functions
    public void OverrideStack(StackedPile stack)
    {
        this.stack = stack;
    }
    public StackedPile DropStack(StackedPile origin_stack)
    {
        if (this.stack.Equals(origin_stack))
        {
            var result = origin_stack.Copy();

            int bigger = Mathf.Max(this.stack.Quantity, origin_stack.Quantity);
            int smaller = Mathf.Min(this.stack.Quantity, origin_stack.Quantity);

            int raw_result = bigger + smaller;
            int diference = raw_result - this.stack.MaxStack;

            //le paso los valores crudos porque internamente se encarga de hacer el recorte
            this.stack.ModifyQuantity(raw_result);
            result.ModifyQuantity(diference);

            return result;
        }
        else
        {
            var result = this.stack.Copy();
            this.stack = origin_stack;
            return result;
        }
    }
    #endregion

    public void Empty()
    {
        stack.Force_to_Empty();
        stack = null;
    }
}

[System.Serializable]
public class StackedPile //La bolsita del bombon
{
    /// <summary>
    /// ///////////////////////////////PONER LO DE LA CALIDAD ACÁ
    /// </summary>


    #region Vars
    [SerializeField] int quant = 0;
    [SerializeField] ElementData element = null; //El Bombon
    [SerializeField] float weight = 0f;
    [SerializeField] int quality = 1;
    public bool IsTabbed = false;
    #endregion

    #region Getters & Setters
    public ElementData Element => element;
    public float Weight => weight;
    public int MaxStack => element.MaxStack;
    public bool IsEmpty => Quantity <= 0 || element == null;
    public bool IsFull => Quantity >= MaxStack;
    public int Quantity
    {
        get => quant;
        set
        {
            quant = value;
            if (quant <= 0)
            {
                element = null;
                quant = 0;
                quality = -1;
            }
            weight = element != null ? quant * element.Weight : 0;
        }
    }
    public int FreeSpaces => MaxStack - quant;
    public bool Full_Element_Is_Equal(ElementData element, int quality)
    {
        return this.element.Equals(element) && this.quality == quality;
    }
    public bool Only_Element_Is_Equal(ElementData element)
    {
        return this.element.Equals(element);
    }
    public int Quality
    {
        get => quality;
        set
        {
            quality = value;
            //quality = Mathf.Clamp(value, 1, element.MaxQuality);
        }
    }
    #endregion

    #region Constructor
    public StackedPile()
    {
        element = null;
        Quantity = 0;
    }
    #endregion

    #region Copy
    public StackedPile Copy()
    {
        StackedPile copy = new StackedPile();
        copy.element = element;
        copy.quant = quant;
        copy.weight = weight;
        return copy;
    }
    #endregion

    #region Modifiers
    public void SetElement(ElementData element)
    {
        this.element = element;
    }

    public void ModifyQuantity(int quant_to_modify)
    {
        if (quant_to_modify > MaxStack) Quantity = MaxStack;
        else Quantity = quant_to_modify;
    }
    #endregion

    #region ADD FUNCTIONS
    public bool Add_SAFE(int quant_to_add = 1)
    {
        var aux = Quantity + quant_to_add;
        if (aux > MaxStack) return false;
        Quantity = aux;
        return true;
    }
    public void Add_RAW(int quant_to_add = 1)
    {
        Quantity = quant_to_add;
        if (Quantity > MaxStack) Quantity = MaxStack;
    }
    public void Add_UNSAFE(int quant_to_add = 1)
    {
        Quantity = quant_to_add;
    }
    #endregion

    #region REMOVE FUNCTIONS
    public bool Remove_SAFE(int quant_to_remove = 1)
    {
        var aux = Quantity - quant_to_remove;
        if (aux < 0) return false;
        Quantity = aux;
        return true;
    }
    public void Remove_RAW(int quant_to_remove = 1)
    {
        Quantity = quant_to_remove;
    }
    #endregion

    #region Fill or Empty
    public void Force_to_Fill()
    {
        Quantity = MaxStack;
    }
    public void Force_to_Empty()
    {
        Quantity = 0;
        element = null;
    }
    #endregion

    #region Object Override
    public override bool Equals(object obj)
    {
        var stack = (StackedPile)obj;
        return stack.element == element && stack.quality == quality;
    }
    public override int GetHashCode()
    {
        var hashCode = 464553162;
        hashCode = hashCode * -1521134295 + quant.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<ElementData>.Default.GetHashCode(element);
        hashCode = hashCode * -1521134295 + weight.GetHashCode();
        hashCode = hashCode * -1521134295 + Weight.GetHashCode();
        hashCode = hashCode * -1521134295 + MaxStack.GetHashCode();
        hashCode = hashCode * -1521134295 + IsEmpty.GetHashCode();
        hashCode = hashCode * -1521134295 + IsFull.GetHashCode();
        hashCode = hashCode * -1521134295 + Quantity.GetHashCode();
        return hashCode;
    }
    #endregion
}

public struct CPResult
{
    [SerializeField] bool process_successfull;
    [SerializeField] int remainder_quantity;
    [SerializeField] string message;
    List<int> slots_equiped;

    public CPResult(bool process_successfull, int remainder_quantity, string message, List<int> slots = default)
    {
        this.process_successfull = process_successfull;
        this.remainder_quantity = remainder_quantity;
        this.message = message;
        this.slots_equiped = slots;
    }

    public void AddSlotsEquiped(List<int> slots) => this.slots_equiped = slots;

    public bool Process_Successfull { get => process_successfull; }
    public int Remainder_Quantity { get => remainder_quantity; }
    public string Message { get => message; }
    public List<int> SlotsEquiped { get => slots_equiped; }
}
