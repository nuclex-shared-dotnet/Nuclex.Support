#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2017 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Nuclex.Support.Collections {

  /// <summary>Variant of BindingList that supports sorting</summary>
  /// <typeparam name="TElement">Type of items the binding list will contain</typeparam>
  public class SortableBindingList<TElement> : BindingList<TElement> {

    #region class PropertyComparer

    /// <summary>Compares two elements based on a single preselected property</summary>
    private class PropertyComparer : IComparer<TElement> {

      /// <summary>Initializes a new property comparer for the specified property</summary>
      /// <param name="property">Property based on which elements should be compared</param>
      /// <param name="direction">Direction in which elements should be sorted</param>
      public PropertyComparer(PropertyDescriptor property, ListSortDirection direction) {
        this.propertyDescriptor = property;

        Type comparerForPropertyType = typeof(Comparer<>).MakeGenericType(property.PropertyType);
        this.comparer = (IComparer)comparerForPropertyType.InvokeMember(
          "Default",
          BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public,
          null, // binder
          null, // target (none since method is static)
          null // argument array
        );

        SetListSortDirection(direction);
      }

      /// <summary>Compares two elements based on the comparer's chosen property</summary>
      /// <param name="first">First element for the comparison</param>
      /// <param name="second">Second element for the comparison</param>
      /// <returns>The relationship of the two elements to each other</returns>
      public int Compare(TElement first, TElement second) {
        return this.comparer.Compare(
          this.propertyDescriptor.GetValue(first),
          this.propertyDescriptor.GetValue(second)
        ) * this.reverse;
      }

      /// <summary>Selects the property based on which elements should be compared</summary>
      /// <param name="descriptor">Descriptor for the property to use for comparison</param>
      private void SetPropertyDescriptor(PropertyDescriptor descriptor) {
        this.propertyDescriptor = descriptor;
      }

      /// <summary>Changes the sort direction</summary>
      /// <param name="direction">New sort direction</param>
      private void SetListSortDirection(ListSortDirection direction) {
        this.reverse = direction == ListSortDirection.Ascending ? 1 : -1;
      }

      /// <summary>Updtes the sorted proeprty and the sort direction</summary>
      /// <param name="descriptor">Property based on which elements will be sorted</param>
      /// <param name="direction">Direction in which elements will be sorted</param>
      public void SetPropertyAndDirection(
        PropertyDescriptor descriptor, ListSortDirection direction
      ) {
        SetPropertyDescriptor(descriptor);
        SetListSortDirection(direction);
      }

      /// <summary>The default comparer for the type of the chosen property</summary>
      private readonly IComparer comparer;
      /// <summary>Descriptor for the chosen property</summary>
      private PropertyDescriptor propertyDescriptor;
      /// <summary>
      ///   Either positive or negative 1 to change the sign of the comparison result
      /// </summary>
      private int reverse;

    }

    #endregion // class PropertyComparer

    /// <summary>Initializes a new BindingList with support for sorting</summary>
    public SortableBindingList() : base(new List<TElement>()) {
      this.comparers = new Dictionary<Type, PropertyComparer>();
    }

    /// <summary>
    ///   Initializes a sortable BindingList, copying the contents of an existing list
    /// </summary>
    /// <param name="enumeration">Existing list whose contents will be shallo-wcopied</param>
    public SortableBindingList(IEnumerable<TElement> enumeration) :
      base(new List<TElement>(enumeration)) {
      this.comparers = new Dictionary<Type, PropertyComparer>();
    }

    /// <summary>
    ///   Used by BindingList implementation to check whether sorting is supported
    /// </summary>
    protected override bool SupportsSortingCore {
      get { return true; }
    }

    /// <summary>
    ///   Used by BindingList implementation to check whether the list is currently sorted
    /// </summary>
    protected override bool IsSortedCore {
      get { return this.isSorted; }
    }

    /// <summary>
    ///   Used by BindingList implementation to track the property the list is sorted by
    /// </summary>
    protected override PropertyDescriptor SortPropertyCore {
      get { return this.propertyDescriptor; }
    }

    /// <summary>
    ///   Used by BindingList implementation to track the direction in which the list is sortd
    /// </summary>
    protected override ListSortDirection SortDirectionCore {
      get { return this.listSortDirection; }
    }

    /// <summary>
    ///   Used by BindingList implementation to check whether the list supports searching
    /// </summary>
    protected override bool SupportsSearchingCore {
      get { return true; }
    }

    /// <summary>
    ///   Used by BindingList implementation to sort the elements in the backing collection
    /// </summary>
    protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction) {

      // Obtain a property comparer that sorts on the attributes the SortableBindingList
      // has been configured for its sort order
      PropertyComparer comparer;
      {
        Type propertyType = property.PropertyType;

        if(!this.comparers.TryGetValue(propertyType, out comparer)) {
          comparer = new PropertyComparer(property, direction);
          this.comparers.Add(propertyType, comparer);
        }

        // Direction may need to be updated
        comparer.SetPropertyAndDirection(property, direction);
      }

      // Check to see if our base class is using a standard List<> in which case
      // we'll sneakily use the downcast to call the List<>.Sort() method, otherwise
      // there's still our own quicksort implementation for IList<>.
      List<TElement> itemsAsList = this.Items as List<TElement>;
      if(itemsAsList != null) {
        itemsAsList.Sort(comparer);
      } else {
        this.Items.QuickSort(0, this.Items.Count, comparer); // from IListExtensions
      }

      this.propertyDescriptor = property;
      this.listSortDirection = direction;
      this.isSorted = true;

      OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
    }

    /// <summary>Used by BindingList implementation to undo any sorting that took place</summary>
    protected override void RemoveSortCore() {
      this.isSorted = false;
      this.propertyDescriptor = base.SortPropertyCore;
      this.listSortDirection = base.SortDirectionCore;

      OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
    }

    /// <summary>
    ///   Used by BindingList implementation to run a search on any of the element's properties
    /// </summary>
    protected override int FindCore(PropertyDescriptor property, object key) {
      int count = this.Count;
      for(int index = 0; index < count; ++index) {
        TElement element = this[index];
        if(property.GetValue(element).Equals(key)) {
          return index;
        }
      }

      return -1;
    }

    /// <summary>Cached property comparers, created for each element property as needed</summary>
    private readonly Dictionary<Type, PropertyComparer> comparers;
    /// <summary>Whether the binding list is currently sorted</summary>
    private bool isSorted;
    /// <summary>Direction in which the binding list is currently sorted</summary>
    private ListSortDirection listSortDirection;
    /// <summary>Descriptor for the property by which the binding list is currently sorted</summary>
    private PropertyDescriptor propertyDescriptor;

  }

} // namespace Nuclex.Support.Collections
